import { ChangeDetectionStrategy, Component, DestroyRef, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { SelectItem } from 'src/app/data/selectItem.interface';
import { BackendService } from 'src/app/services/backend/backend.service';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-exercise-edit',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './exercise-edit.component.html',
  styleUrls: ['./exercise-edit.component.css']
})
export class ExerciseEditComponent implements OnInit {
  private backendService = inject(BackendService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private fb = inject(FormBuilder);
  private destroyRef = inject(DestroyRef);

  private codeNum = 0;
  private exerciseDbId = 0;

  muscles = signal<SelectItem[]>([]);
  tags = signal<SelectItem[]>([]);
  editExerciseForm!: FormGroup;

  difficultOptions = [
    { value: 1, text: 'Easy' },
    { value: 2, text: 'Medium' },
    { value: 3, text: 'Hard' },
  ];

  ngOnInit(): void {
    this.codeNum = Number(this.route.snapshot.paramMap.get('id')) || 0;

    this.editExerciseForm = this.fb.group({
      name: [null, Validators.required],
      code: [null, Validators.required],
      selectedMuscles: [null, Validators.required],
      selectedTags: [null, Validators.required],
      startDescription: [null],
      executionDescription: [null, Validators.required],
      adviceDescription: [null, Validators.required],
      difficultType: [1, Validators.required],
    });

    this.backendService.getExerciseEditParams('en')
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: params => {
          this.muscles.set(params.allMuscles);
          this.tags.set(params.allTags);

          if (this.codeNum) {
            this.backendService.getExerciseDetails(this.codeNum, 'en')
              .pipe(takeUntilDestroyed(this.destroyRef))
              .subscribe({
                next: exercise => {
                  this.exerciseDbId = exercise.id;
                  this.editExerciseForm.patchValue({
                    name: exercise.name,
                    code: exercise.codeNum,
                    startDescription: exercise.startingPositionDescription,
                    executionDescription: exercise.executionDescription,
                    adviceDescription: exercise.adviceDescription,
                    selectedMuscles: this.muscles().filter(m => exercise.muscles.includes(parseInt(m.value))),
                    selectedTags: this.tags().filter(t => exercise.tags.includes(parseInt(t.value))),
                    difficultType: exercise.difficultType,
                  });
                },
                error: (err) => console.error('Failed to fetch exercise details', err)
              });
          } else {
            this.editExerciseForm.controls['code'].setValue(params.offeredCode);
          }
        },
        error: (err) => console.error('Failed to fetch exercise edit params', err)
      });
  }

  trackByValue(index: number, item: SelectItem): string {
    return item.value;
  }

  trackByOptionValue(index: number, option: { value: number }): number {
    return option.value;
  }

  save(): void {
    const v = this.editExerciseForm.value;
    const payload = {
      id: this.exerciseDbId,
      name: v.name,
      codeNum: v.code,
      startingPositionDescription: v.startDescription,
      executionDescription: v.executionDescription,
      adviceDescription: v.adviceDescription,
      difficultType: v.difficultType,
      muscles: v.selectedMuscles?.map((m: any) => parseInt(m.value)) ?? [],
      tags: v.selectedTags?.map((t: any) => parseInt(t.value)) ?? [],
    };

    const request$ = this.exerciseDbId === 0
      ? this.backendService.createExercise(payload)
      : this.backendService.editExercise(this.exerciseDbId, payload);

    request$
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => this.router.navigate(['/exercises']),
        error: (err) => console.error('Failed to save exercise', err)
      });
  }
}

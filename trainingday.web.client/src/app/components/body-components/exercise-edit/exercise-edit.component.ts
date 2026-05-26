import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { SelectItem } from 'src/app/data/selectItem.interface';
import { BackendService } from 'src/app/services/backend/backend.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-exercise-edit',
  templateUrl: './exercise-edit.component.html',
  styleUrls: ['./exercise-edit.component.css']
})
export class ExerciseEditComponent implements OnInit {
  private codeNum = 0;
  private exerciseDbId = 0;

  muscles!: SelectItem[];
  tags!: SelectItem[];
  editExerciseForm!: FormGroup;

  constructor(
    private backendService: BackendService,
    private router: Router,
    private route: ActivatedRoute,
    private fb: FormBuilder
  ) {}

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

    this.backendService.getExerciseEditParams('en').subscribe(params => {
      this.muscles = params.allMuscles;
      this.tags = params.allTags;

      if (this.codeNum) {
        this.backendService.getExerciseDetails(this.codeNum, 'en').subscribe(exercise => {
          this.exerciseDbId = exercise.id;
          this.editExerciseForm.patchValue({
            name: exercise.name,
            code: exercise.codeNum,
            startDescription: exercise.startingPositionDescription,
            executionDescription: exercise.executionDescription,
            adviceDescription: exercise.adviceDescription,
            selectedMuscles: this.muscles.filter(m => exercise.muscles.includes(parseInt(m.value))),
            selectedTags: this.tags.filter(t => exercise.tags.includes(parseInt(t.value))),
            difficultType: exercise.difficultType,
          });
        });
      } else {
        this.editExerciseForm.controls['code'].setValue(params.offeredCode);
      }
    });
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

    if (this.exerciseDbId === 0) {
      this.backendService.createExercise(payload).subscribe(() => {
        this.router.navigate(['/exercises']);
      });
    } else {
      this.backendService.editExercise(this.exerciseDbId, payload).subscribe(() => {
        this.router.navigate(['/exercises']);
      });
    }
  }
}

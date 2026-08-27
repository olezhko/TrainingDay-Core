import { ChangeDetectionStrategy, Component, DestroyRef, OnInit, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CommonModule } from '@angular/common';
import { Router, ActivatedRoute } from '@angular/router';
import { BackendService } from 'src/app/services/backend/backend.service';
import { ExerciseDetails } from 'src/app/data/exercises/exercise-details.model';
import { ExerciseTagsEnglishLabels, ExerciseTags } from 'src/app/data/exercises/exercise-tags';
import { MusclesEnumEnglishLabels, MusclesEnum } from 'src/app/data/exercises/exercise-muscle';
import { environment } from '../../../../environment/environment';
import { AuthService } from 'src/app/services/auth/auth.service';

@Component({
  selector: 'app-exercise-item',
  standalone: true,
  imports: [CommonModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './exercise-item.component.html',
  styleUrls: ['./exercise-item.component.css']
})
export class ExerciseItemComponent implements OnInit {
  private backendService = inject(BackendService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private destroyRef = inject(DestroyRef);
  public authService = inject(AuthService);

  exerciseId = 0;
  exercise = signal<ExerciseDetails>(new ExerciseDetails());
  imageSrc = signal(' ');

  ngOnInit() {
    this.exerciseId = Number(this.route.snapshot.paramMap.get('id'));

    this.backendService.getExerciseDetails(this.exerciseId, "en")
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (data: ExerciseDetails) => {
          this.exercise.set(data);
          this.imageSrc.set(environment.baseUrl + `/exercise_images/${data.codeNum}.jpg`);
        },
        error: (err) => {
          console.error('Failed to fetch exercise details', err);
        }
      });
  }

  tagLabel(tag: string): string {
    return ExerciseTagsEnglishLabels[tag as ExerciseTags] ?? tag;
  }

  muscleLabel(muscle: string): string {
    return MusclesEnumEnglishLabels[muscle as MusclesEnum] ?? muscle;
  }

  difficultLabel(value: number): string {
    return { 1: 'Easy', 2: 'Medium', 3: 'Hard' }[value] ?? 'Unknown';
  }

  difficultClass(value: number): string {
    return { 1: 'diff-easy', 2: 'diff-medium', 3: 'diff-hard' }[value] ?? '';
  }

  trackByValue(index: number, value: string): string {
    return value;
  }

  edit(): void {
    this.router.navigate(['/exercise/edit', this.exerciseId]);
  }

  delete(): void {
    this.backendService.deleteExercise(this.exerciseId)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        complete: () => this.router.navigate(['/exercises'])
      });
  }
}

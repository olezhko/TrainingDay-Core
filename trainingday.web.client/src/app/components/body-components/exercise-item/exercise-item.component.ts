import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { BackendService } from 'src/app/services/backend/backend.service';
import { ExerciseDetails } from 'src/app/data/exercises/exercise-details.model';
import { ExerciseTagsEnglishLabels, ExerciseTags } from 'src/app/data/exercises/exercise-tags';
import { MusclesEnumEnglishLabels, MusclesEnum } from 'src/app/data/exercises/exercise-muscle';
import { environment } from '../../../../environment/environment';
import { AuthService } from 'src/app/services/auth/auth.service';

@Component({
  selector: 'app-exercise-item',
  templateUrl: './exercise-item.component.html',
  styleUrls: ['./exercise-item.component.css']
})
export class ExerciseItemComponent implements OnInit {

  exerciseId: number = 0;
  exercise: ExerciseDetails;
  imageSrc: string = ' ';
  constructor(private backendService: BackendService, private router: Router, private route: ActivatedRoute, public authService: AuthService) {
    this.exercise = new ExerciseDetails;
  }

  ngOnInit() {
    this.exerciseId = Number(this.route.snapshot.paramMap.get('id'));

    this.backendService.getExerciseDetails(this.exerciseId, "en").subscribe({
      next: (data: ExerciseDetails) => {

        this.exercise = data;
        this.imageSrc = environment.baseUrl + `/exercise_images/${this.exercise.codeNum}.jpg`;

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

  edit(): void {
    this.router.navigate(['/exercise/edit', this.exerciseId]);
  }

  delete(): void {
    this.backendService.deleteExercise(this.exerciseId).subscribe({
      complete: () => this.router.navigate(['/exercises'])
    });
  }
}

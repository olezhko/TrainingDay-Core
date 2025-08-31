import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { BackendService } from 'src/app/services/backend/backend.service';
import { ExerciseDetails } from 'src/app/data/exercises/exercise-details.model';
import { ExerciseTagsEnglishLabels, ExerciseTags } from 'src/app/data/exercises/exercise-tags';
import { MusclesEnumEnglishLabels, MusclesEnum } from 'src/app/data/exercises/exercise-muscle';
import { environment } from '../../../../environment/environment'

@Component({
  selector: 'app-exercise-item',
  templateUrl: './exercise-item.component.html',
  styleUrls: ['./exercise-item.component.css']
})
export class ExerciseItemComponent implements OnInit {

  exerciseId: number = 0;
  exercise: ExerciseDetails;
  imageSrc: string = ' ';
  constructor(private backendService: BackendService, private router: Router, private route: ActivatedRoute) {
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
}

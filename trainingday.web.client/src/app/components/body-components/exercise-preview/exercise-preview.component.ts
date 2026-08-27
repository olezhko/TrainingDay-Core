import { ChangeDetectionStrategy, Component, computed, inject, input } from '@angular/core';
import { ExercisePreview } from 'src/app/data/exercises/exercise-preview.model';
import { Router } from '@angular/router';
import { environment } from '../../../../environment/environment';

@Component({
  selector: 'app-exercise-preview',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './exercise-preview.component.html',
  styleUrls: ['./exercise-preview.component.css']
})
export class ExercisePreviewComponent {
  private router = inject(Router);

  exercise = input.required<ExercisePreview>();

  imageSrc = computed(() => environment.baseUrl + `/exercise_images/${this.exercise().codeNum}.jpg`);

  showDetails() {
    this.router.navigate(['exercise/details/' + this.exercise().codeNum]);
  }
}

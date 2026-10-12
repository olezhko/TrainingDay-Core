import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import { ExercisePreview } from 'src/app/data/exercises/exercise-preview.model';
import { RouterLink } from '@angular/router';
import { environment } from '../../../../environment/environment';

@Component({
  selector: 'app-exercise-preview',
  standalone: true,
  imports: [RouterLink],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './exercise-preview.component.html',
  styleUrls: ['./exercise-preview.component.css']
})
export class ExercisePreviewComponent {
  exercise = input.required<ExercisePreview>();

  imageSrc = computed(() => environment.baseUrl + `/exercise_images/${this.exercise().codeNum}.jpg`);
}

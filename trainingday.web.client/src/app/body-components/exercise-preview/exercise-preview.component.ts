import { Input, Component, OnChanges, SimpleChanges } from '@angular/core';
import { ExercisePreview } from 'src/app/data/exercises/exercise-preview.model';

@Component({
  selector: 'app-exercise-preview',
  templateUrl: './exercise-preview.component.html',
  styleUrls: ['./exercise-preview.component.css']
})
export class ExercisePreviewComponent implements OnChanges{
  ngOnChanges(changes: SimpleChanges): void {
    this.imageSrc = `https://localhost:7081/exercise_images/${this.exercise.codeNum}.jpg`;
  }
  @Input() exercise: ExercisePreview = new ExercisePreview(12, "123", 12);

  imageSrc: string = ' ';

  showDetails() {
    // Handle the details button click
    console.log('Details button clicked for exercise:', this.exercise);
    // Add your logic to navigate to details page or show details modal, etc.
  }
}

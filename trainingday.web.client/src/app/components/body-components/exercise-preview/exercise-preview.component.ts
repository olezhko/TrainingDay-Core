import { Input, Component, OnChanges, SimpleChanges } from '@angular/core';
import { ExercisePreview } from 'src/app/data/exercises/exercise-preview.model';
import { Router } from '@angular/router';
import { BackendService } from 'src/app/services/backend/backend.service';

@Component({
  selector: 'app-exercise-preview',
  templateUrl: './exercise-preview.component.html',
  styleUrls: ['./exercise-preview.component.css']
})
export class ExercisePreviewComponent implements OnChanges {

  constructor(private backendService: BackendService, private router: Router) { }

  ngOnChanges(changes: SimpleChanges): void {
    this.imageSrc = `https://localhost:7081/exercise_images/${this.exercise.codeNum}.jpg`;
  }
  @Input() exercise: ExercisePreview = new ExercisePreview(12, "123", 12);

  imageSrc: string = ' ';

  showDetails() {
    this.router.navigate(['exercise/details/' + this.exercise.codeNum]);
  }
}

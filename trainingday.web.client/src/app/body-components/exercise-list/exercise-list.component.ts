import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ExercisePreview } from 'src/app/data/exercises/exercise-preview.model';
import { BackendService } from 'src/app/services/backend/backend.service';

@Component({
  selector: 'app-exercise-list',
  templateUrl: './exercise-list.component.html',
  styleUrls: ['./exercise-list.component.css']
})
export class ExerciseListComponent implements OnInit  {
  public muscleFilter: number | undefined;
  public textPattern: string = '';

  public exercisePreviews: ExercisePreview[] = [];
  constructor(private backendService: BackendService, private router: Router)  {  }

  ngOnInit(): void {
    this.backendService.getExercises(this.muscleFilter,this.textPattern,'en').subscribe(exercises => {
      this.exercisePreviews = exercises;
    });
  }

  createNew() : void{
    this.router.navigate(['/exercise/new']);
  }
}

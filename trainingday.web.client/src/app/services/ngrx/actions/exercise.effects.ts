import { Injectable } from '@angular/core';
import { Actions, ofType, createEffect } from '@ngrx/effects';
import { of } from 'rxjs';
import { map, switchMap, catchError } from 'rxjs/operators';
import * as ExerciseActions from './exercise.actions';
import { BackendService } from '../../backend/backend.service';

@Injectable()
export class ExerciseEffects {
  loadExercises$ = createEffect(() => this.actions$.pipe(
    ofType(ExerciseActions.loadExercises),
    switchMap(() => this.exerciseService.getExercises(selectedMuscle, filterName, twoLetterCulture).pipe(
      map(exercises => ExerciseActions.exercisesLoaded({ exercises })),
      catchError(() => of({ type: '[Exercise] Load Exercises Error' }))
    ))
  ));
  
  constructor(
    private actions$: Actions,
    private exerciseService: BackendService
  ) {}
}
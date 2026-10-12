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
    switchMap(({ selectedMuscles, selectedTags, selectedDifficulty, filterName, twoLetterCulture }) =>
      this.exerciseService.getExercises(selectedMuscles, selectedTags, selectedDifficulty, filterName, twoLetterCulture).pipe(
        map(exercises => ExerciseActions.exercisesLoaded({ exercises })),
        catchError(error => of(ExerciseActions.exercisesLoadError({ error })))
      ))
  ));

  constructor(
    private actions$: Actions,
    private exerciseService: BackendService
  ) {}
}

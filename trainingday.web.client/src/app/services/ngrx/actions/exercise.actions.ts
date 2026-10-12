import { createAction, props } from '@ngrx/store';
import { ExercisePreview } from 'src/app/data/exercises/exercise-preview.model';

export const loadExercises = createAction(
  '[Exercise] Load',
  props<{
    selectedMuscles: number[];
    selectedTags: number[];
    selectedDifficulty: number[];
    filterName: string;
    twoLetterCulture: string;
  }>()
);

export const exercisesLoaded = createAction(
  '[Exercise] Exercises Loaded',
  props<{ exercises: ExercisePreview[] }>()
);

export const exercisesLoadError = createAction(
  '[Exercise] Exercises Load Error',
  props<{ error: unknown }>()
);

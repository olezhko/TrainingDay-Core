import { createAction, props } from '@ngrx/store';
import { ExercisePreview } from 'src/app/data/exercises/exercise-preview.model';

export const loadExercises = createAction('[Exercise] Load Exercises');
export const exercisesLoaded = createAction('[Exercise] Exercises Loaded', props<{ exercises: ExercisePreview[] }>());
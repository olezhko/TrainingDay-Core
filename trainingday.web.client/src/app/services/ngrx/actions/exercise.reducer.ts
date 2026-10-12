import { createReducer, on } from '@ngrx/store';
import { ExercisePreview } from 'src/app/data/exercises/exercise-preview.model';
import * as ExerciseActions from './exercise.actions';

export interface ExerciseState {
  exercises: ExercisePreview[];
  isLoading: boolean;
  hasMore: boolean;
  searchQuery: string | null;
}

export const initialState: ExerciseState = {
  exercises: [],
  isLoading: false,
  hasMore: true,
  searchQuery: null
};

export const exerciseReducer = createReducer(
  initialState,
  on(ExerciseActions.loadExercises, state => ({
    ...state,
    isLoading: true
  })),
  on(ExerciseActions.exercisesLoaded, (state, { exercises }) => ({
    ...state,
    exercises,
    isLoading: false
  })),
  on(ExerciseActions.exercisesLoadError, state => ({
    ...state,
    isLoading: false
  }))
);

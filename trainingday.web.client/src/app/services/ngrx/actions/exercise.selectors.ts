import { createSelector, createFeatureSelector } from '@ngrx/store';
import { ExerciseState } from './exercise.reducer';

export const selectExerciseState = createFeatureSelector<ExerciseState>('exercise');

export const selectExerciseList = createSelector(
  selectExerciseState,
  state => state.exercises
);

export const selectIsLoading = createSelector(
  selectExerciseState,
  state => state.isLoading
);

export const selectHasMore = createSelector(
  selectExerciseState,
  state => state.hasMore
);
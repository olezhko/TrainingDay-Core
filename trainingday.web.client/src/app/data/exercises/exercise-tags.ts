export enum ExerciseTags {
  CanDoAtHome = '0',
  ExerciseByTime = '1',
  ExerciseByDistance = '2',
  ExerciseByRepsAndWeight = '3',
  BarbellExist = '4',
  DumbbellExist = '5',
  BenchExist = '6',
  DatabaseExercise = '7',
  ExerciseByReps = '8',
}

export const ExerciseTagsEnglishLabels: Record<ExerciseTags, string> = {
  [ExerciseTags.CanDoAtHome]: 'No inventory',
  [ExerciseTags.ExerciseByTime]: 'By Time',
  [ExerciseTags.ExerciseByDistance]: 'By Distance',
  [ExerciseTags.ExerciseByRepsAndWeight]: 'Repetitions And Weight',
  [ExerciseTags.BarbellExist]: 'Barbell',
  [ExerciseTags.DumbbellExist]: 'Dumbbell',
  [ExerciseTags.BenchExist]: 'Bench',
  [ExerciseTags.DatabaseExercise]: 'Default',
  [ExerciseTags.ExerciseByReps]: 'Repetitions',
};

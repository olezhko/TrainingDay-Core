﻿namespace TrainingDay.Common.Models
{
    public class RepositoryBase
    {
        public IEnumerable<Training> Trainings { get; set; }
        public IEnumerable<Exercise> Exercises { get; set; }
        public IEnumerable<TrainingExerciseComm> TrainingExercise { get; set; }
        public IEnumerable<TrainingUnion> TrainingUnions { get; set; }
        public IEnumerable<SuperSet> SuperSets { get; set; }
        public IEnumerable<LastTraining> LastTrainings { get; set; }
        public IEnumerable<LastTrainingExercise> LastTrainingExercises { get; set; }
        public IEnumerable<WeightNote> BodyControl { get; set; }
    }

    public class TrainingUnion
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TrainingIDsString { get; set; }
        public bool IsExpanded { get; set; }
    }

    public class WeightNote
    {
        public int Id { get; set; }
        public double Weight { get; set; }
        public DateTime Date { get; set; }
        public int Type { get; set; }
    }

    public class TrainingExerciseComm
    {
        public int Id { get; set; }

        public int TrainingId { get; set; }
        public int ExerciseId { get; set; }
        public int OrderNumber { get; set; }
        public int SuperSetId { get; set; }
        public string WeightAndRepsString { get; set; }
    }

    public class BaseExercise
    {
        public string Name { get; set; }
        public Description Description { get; set; }
        public string MusclesString { get; set; } // text, by "," enum collection
        public string Tags { get; set; }
        public int CodeNum { get; set; }
        public DifficultTypes DifficultLevel { get; set; }
    }

    public class Training
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }

    public class Exercise 
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string MusclesString { get; set; } // text, by "," enum collection
        public int TagsValue { get; set; }
        public int CodeNum { get; set; }
        public DifficultTypes DifficultLevel { get; set; }
    }

    public class LastTraining
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime Time { get; set; }
        public TimeSpan ElapsedTime { get; set; }
        public int TrainingId { get; set; }
    }

    public class LastTrainingExercise
    {
        public int Id { get; set; }
        public int LastTrainingId { get; set; }
        public string ExerciseName { get; set; }
        public string Description { get; set; }
        public int OrderNumber { get; set; }
        public int SuperSetId { get; set; }

        public string MusclesString { get; set; }
        public string WeightAndRepsString { get; set; }
        public int TagsValue { get; set; }
        public int CodeNum { get; set; }
    }

    public class SuperSet
    {
        public int Id { get; set; }
        public int Count { get; set; }
        public int TrainingId { get; set; }
    }

    public enum DifficultTypes
    {
        Easy = 1,
        Medium = 2,
        Hard = 3,
    }
}

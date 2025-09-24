using TrainingDay.Common.Extensions;
using TrainingDay.Web.Entities;

namespace TrainingDay.Web.Services.Exercises.Models
{
    public class WorkoutExercise
    {
        public required int ExerciseId { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required int Code { get; set; }
        public required IReadOnlyCollection<ExerciseTag> Tags { get; set; }
        public required IReadOnlyCollection<ExerciseMuscle> Muscles { get; set; }
    }

    public static class WorkoutExerciseExtension
    {
        public static WorkoutExercise ToDomain(this WebExercise source)
        {
            return new WorkoutExercise()
            {
                Code = source.CodeNum,
                Description = source.Description,
                ExerciseId = source.Id,
                Muscles = ParseMuscles(source.MusclesString),
                Name = source.Name,
                Tags = ParseTags(source.TagsValue),
            }; 
        }

        private static IReadOnlyCollection<ExerciseTag> ParseTags(int tagsValue)
        {
            return [.. ExerciseExtensions.ConvertTagIntToList(tagsValue)
                .Select(tag => new ExerciseTag()
                {
                    Id = (int)tag,
                    Name = tag.ToString()!,
                })];
        }

        private static IReadOnlyCollection<ExerciseMuscle> ParseMuscles(string musclesString)
        {
            return [.. ExerciseExtensions.ConvertMuscleStringToList(musclesString)
                .Select(muscle => new ExerciseMuscle()
                {
                    Id = (int)muscle,
                    Name = muscle.ToString()!,
                })];
        }
    }
}

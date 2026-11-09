using Newtonsoft.Json;
using TrainingDay.Common.Extensions;
using TrainingDay.Common.Models;
using TrainingDay.Web.Entities;
using TrainingDay.Web.Services.Exercises.Models;

namespace TrainingDay.Web.Services.Mapping;

public static class ExerciseMappingExtensions
{
    /// <summary>Maps the entity; <see cref="WebExercise.Culture"/> must be loaded to fill the culture code.</summary>
    public static ExerciseModel ToModel(this WebExercise entity)
    {
        Description? description = entity.Description is null ? null : JsonConvert.DeserializeObject<Description>(entity.Description);

        return new ExerciseModel
        {
            Id = entity.Id,
            CodeNum = entity.CodeNum,
            Name = entity.Name,
            StartingPosition = description?.StartPosition,
            Execution = description?.Execution,
            Advice = description?.Advice,
            Muscles = ExerciseExtensions.ConvertMuscleStringToList(entity.MusclesString),
            Tags = [.. ExerciseExtensions.ConvertTagIntToList(entity.TagsValue)],
            DifficultType = entity.DifficultType,
            CultureCode = entity.Culture?.Code,
        };
    }

    /// <summary>Copies the editable fields onto the entity; the id and culture are left as they are.</summary>
    public static void ApplyTo(this ExerciseModel model, WebExercise entity)
    {
        entity.Name = model.Name;
        entity.CodeNum = model.CodeNum;
        entity.Description = JsonConvert.SerializeObject(new Description
        {
            StartPosition = model.StartingPosition,
            Execution = model.Execution,
            Advice = model.Advice,
        });
        entity.MusclesString = ExerciseExtensions.ConvertFromMuscleListToString([.. model.Muscles]);
        entity.TagsValue = ExerciseExtensions.ConvertTagListToInt(model.Tags);
        entity.DifficultType = model.DifficultType;
    }
}

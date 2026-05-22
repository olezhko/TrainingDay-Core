using Newtonsoft.Json;
using TrainingDay.Common.Extensions;
using TrainingDay.Common.Models;
using TrainingDay.Web.Entities;
using TrainingDay.Web.Server.ViewModels.Exercises;

namespace TrainingDay.Web.Server.Mapper;

public static class ExerciseMappingExtensions
{
    public static ExerciseViewModel ToViewModel(this WebExercise entity) => new ExerciseViewModel(entity);

    public static WebExercise ToEntity(this ExerciseViewModel vm) => new WebExercise
    {
        Id = vm.Id,
        Name = vm.Name,
        CodeNum = vm.CodeNum,
        Description = JsonConvert.SerializeObject(new Description
        {
            StartPosition = vm.StartingPositionDescription,
            Advice = vm.AdviceDescription,
            Execution = vm.ExecutionDescription,
        }),
        MusclesString = ExerciseExtensions.ConvertFromMuscleListToString(vm.Muscles.ToList()),
        TagsValue = ExerciseExtensions.ConvertTagListToInt(vm.Tags.ToList()),
    };
}

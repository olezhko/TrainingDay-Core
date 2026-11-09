using System.Collections.ObjectModel;
using TrainingDay.Common.Models;
using TrainingDay.Web.Server.ViewModels.Exercises;
using TrainingDay.Web.Services.Exercises.Models;

namespace TrainingDay.Web.Server.Mapper;

public static class ExerciseMappingExtensions
{
    public static ExerciseViewModel ToViewModel(this ExerciseModel model) => new ExerciseViewModel
    {
        Id = model.Id,
        Name = model.Name,
        CodeNum = model.CodeNum,
        StartingPositionDescription = model.StartingPosition,
        ExecutionDescription = model.Execution,
        AdviceDescription = model.Advice,
        Muscles = new ObservableCollection<MusclesEnum>(model.Muscles),
        Tags = [.. model.Tags],
        CultureCode = model.CultureCode,
        DifficultType = model.DifficultType,
    };

    public static ExerciseModel ToModel(this ExerciseViewModel vm) => new ExerciseModel
    {
        Id = vm.Id,
        Name = vm.Name,
        CodeNum = vm.CodeNum,
        StartingPosition = vm.StartingPositionDescription,
        Execution = vm.ExecutionDescription,
        Advice = vm.AdviceDescription,
        Muscles = [.. vm.Muscles],
        Tags = [.. vm.Tags],
        CultureCode = vm.CultureCode,
        DifficultType = vm.DifficultType,
    };

    public static WorkoutExerciseResponse ToResponse(this WorkoutExercise model) => new WorkoutExerciseResponse(
        model.ExerciseId,
        model.Name,
        model.Description,
        model.Code,
        [.. model.Tags.Select(tag => new NamedValueResponse(tag.Id, tag.Name))],
        [.. model.Muscles.Select(muscle => new NamedValueResponse(muscle.Id, muscle.Name))]);
}

using TrainingDay.Common.Models;
using TrainingDay.Web.Entities.MobileItems;

namespace TrainingDay.Web.Services.Mapping;

public static class UserRepoMappingExtensions
{
    public static Exercise ToModel(this UserExercise src) => new()
    {
        Id = src.DatabaseId,
        Description = src.Description,
        Name = src.Name,
        MusclesString = src.MusclesString,
        TagsValue = src.TagsValue,
        CodeNum = src.CodeNum,
    };

    public static WeightNote ToModel(this UserWeightNote src) => new()
    {
        Id = src.DatabaseId,
        Date = src.Date,
        Weight = src.Weight,
        Type = src.Type,
    };

    public static TrainingExercise ToModel(this UserTrainingExercise src) => new()
    {
        Id = src.DatabaseId,
        TrainingId = src.TrainingId,
        ExerciseId = src.ExerciseId,
        SuperSetId = src.SuperSetId,
        WeightAndRepsString = src.WeightAndRepsString,
        OrderNumber = src.OrderNumber,
    };

    public static Training ToModel(this UserTraining src) => new()
    {
        Id = src.DatabaseId,
        Title = src.Title,
    };

    public static SuperSet ToModel(this UserSuperSet src) => new()
    {
        Id = src.DatabaseId,
        TrainingId = src.TrainingId,
        Count = src.Count,
    };

    public static LastTrainingExercise ToModel(this UserLastTrainingExercise src) => new()
    {
        Id = src.DatabaseId,
        LastTrainingId = src.LastTrainingId,
        ExerciseName = src.ExerciseName,
        SuperSetId = src.SuperSetId,
        WeightAndRepsString = src.WeightAndRepsString,
        OrderNumber = src.OrderNumber,
        Description = src.Description,
        MusclesString = src.MusclesString,
        TagsValue = src.TagsValue,
    };

    public static LastTraining ToModel(this UserLastTraining src) => new()
    {
        Id = src.DatabaseId,
        TrainingId = src.TrainingId,
        Title = src.Title,
        Time = src.Time,
        ElapsedTime = src.ElapsedTime,
    };

    public static TrainingUnion ToModel(this UserTrainingGroup src) => new()
    {
        Id = src.DatabaseId,
        Name = src.Name,
        TrainingIDsString = src.TrainingIDsString,
    };
}

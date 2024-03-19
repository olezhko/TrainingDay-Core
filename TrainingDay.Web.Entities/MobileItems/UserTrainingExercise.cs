using System.ComponentModel.DataAnnotations;

namespace TrainingDay.Web.Entities.MobileItems;

public class UserTrainingExercise : TrainingDay.Common.TrainingExerciseComm, IUserEntity
{
    public UserTrainingExercise()
    {

    }
    public UserTrainingExercise(TrainingDay.Common.TrainingExerciseComm item)
    {
        DatabaseId = item.Id;
        ExerciseId = item.ExerciseId;
        OrderNumber = item.OrderNumber;
        TrainingId = item.TrainingId;
        WeightAndRepsString = item.WeightAndRepsString;
        SuperSetId = item.SuperSetId;
    }

    [Key]
    public new int Id { get; set; }

    public Guid UserId { get; set; }
    public int DatabaseId { get; set; }

    public virtual User User { get; set; }
}
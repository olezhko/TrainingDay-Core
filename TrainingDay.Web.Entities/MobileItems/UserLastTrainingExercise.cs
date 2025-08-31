using System.ComponentModel.DataAnnotations;

namespace TrainingDay.Web.Entities.MobileItems;

public class UserLastTrainingExercise : Common.Models.LastTrainingExercise, IUserEntity
{
    public UserLastTrainingExercise()
    {

    }
    public UserLastTrainingExercise(Common.Models.LastTrainingExercise item)
    {
        DatabaseId = item.Id;
        Description = item.Description;
        LastTrainingId = item.LastTrainingId;
        WeightAndRepsString = item.WeightAndRepsString;
        OrderNumber = item.OrderNumber;
        MusclesString = item.MusclesString;
        ExerciseName = item.ExerciseName;
        SuperSetId = item.SuperSetId;
    }
    [Key]
    public new int Id { get; set; }

    public Guid UserId { get; set; }
    public int DatabaseId { get; set; }

    public virtual MobileUser User { get; set; }

}
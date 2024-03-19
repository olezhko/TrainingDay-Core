using System.ComponentModel.DataAnnotations;

namespace TrainingDay.Web.Entities.MobileItems;

public class UserLastTraining : TrainingDay.Common.LastTraining, IUserEntity
{
    public UserLastTraining()
    {

    }
    public UserLastTraining(TrainingDay.Common.LastTraining item)
    {
        ElapsedTime = item.ElapsedTime;
        DatabaseId = item.Id;
        TrainingId = item.TrainingId;
        Time = item.Time;
        Title = item.Title;
    }
    [Key]
    public new int Id { get; set; }

    public Guid UserId { get; set; }
    public int DatabaseId { get; set; }

    public virtual User User { get; set; }
}
using System.ComponentModel.DataAnnotations;

namespace TrainingDay.Web.Entities.MobileItems;

public class UserTraining : Common.Models.Training, IUserEntity
{
    public UserTraining()
    {

    }
    public UserTraining(Common.Models.Training training)
    {
        DatabaseId = training.Id;
        Title = training.Title;
    }

    [Key]
    public new int Id { get; set; }

    public Guid UserId { get; set; }
    public int DatabaseId { get; set; }

    public virtual MobileUser User { get; set; }
}
using System.ComponentModel.DataAnnotations;

namespace TrainingDay.Web.Entities.MobileItems;

public class UserTrainingGroup : TrainingDay.Common.TrainingUnion, IUserEntity
{
    public UserTrainingGroup()
    {

    }

    public UserTrainingGroup(TrainingDay.Common.TrainingUnion item)
    {
        DatabaseId = item.Id;
        Name = item.Name;
        TrainingIDsString = item.TrainingIDsString;
        IsExpanded = item.IsExpanded;
    }

    [Key]
    public new int Id { get; set; }
    public Guid UserId { get; set; }
    public int DatabaseId { get; set; }

    public virtual User User { get; set; }
}

using System.ComponentModel.DataAnnotations;

namespace TrainingDay.Web.Entities.MobileItems;

public class UserSuperSet : TrainingDay.Common.SuperSet, IUserEntity
{
    public UserSuperSet()
    {

    }
    public UserSuperSet(TrainingDay.Common.SuperSet superSet)
    {
        TrainingId = superSet.TrainingId;
        Count = superSet.Count;
        DatabaseId = superSet.Id;
    }

    [Key]
    public new int Id { get; set; }

    public Guid UserId { get; set; }
    public int DatabaseId { get; set; }

    public virtual User User { get; set; }
}
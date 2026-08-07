using System.ComponentModel.DataAnnotations;

namespace TrainingDay.Web.Entities.MobileItems;

public class UserWeightNote : Common.Models.WeightNote, IUserEntity
{
    public UserWeightNote()
    {

    }
    public UserWeightNote(Common.Models.WeightNote item)
    {
        DatabaseId = item.Id;
        Date = item.Date;
        Weight = item.Weight;
        Type = item.Type;
    }

    [Key]
    public new int Id { get; set; }

    public Guid UserId { get; set; }
    public int DatabaseId { get; set; }

    public virtual MobileUser User { get; set; }
}
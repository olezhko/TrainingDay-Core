using System.ComponentModel.DataAnnotations;

namespace TrainingDay.Web.Entities.MobileItems;

public class UserMobileToken : AuditableEntity
{
    [Key]
    public int Id { get; set; }
    public int TokenId { get; set; }
    public Guid UserId { get; set; }

    public virtual MobileUser User { get; set; }

    public virtual MobileToken Token { get; set; }
}
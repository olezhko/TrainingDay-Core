using System.ComponentModel.DataAnnotations;

namespace TrainingDay.Web.Entities.MobileItems;

public class SocialWorkoutLike
{
    [Key]
    public int Id { get; set; }

    public int SocialWorkoutId { get; set; }
    public Guid UserId { get; set; }
    public DateTime Created { get; set; }

    public virtual SocialWorkout SocialWorkout { get; set; }
    public virtual MobileUser User { get; set; }
}

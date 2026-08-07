using System.ComponentModel.DataAnnotations;

namespace TrainingDay.Web.Entities.MobileItems;

public class SocialWorkout : AuditableEntity
{
    [Key]
    public int Id { get; set; }

    public Guid OwnerUserId { get; set; }
    public required string OwnerNickname { get; set; }

    public required string WorkoutName { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan Duration { get; set; }

    public virtual MobileUser OwnerUser { get; set; }
    public virtual ICollection<SocialWorkoutExercise> Exercises { get; set; } = new List<SocialWorkoutExercise>();
    public virtual ICollection<SocialWorkoutLike> Likes { get; set; } = new List<SocialWorkoutLike>();
}

using Microsoft.AspNetCore.Identity;
using TrainingDay.Web.Entities.MobileItems;

namespace TrainingDay.Web.Entities;

public class MobileUser : IdentityUser<Guid>
{
    public bool ShareCompletedWorkouts { get; set; } = true;

    public virtual UserMobileToken UserMobileToken { get; set; }

    public virtual ICollection<UserSuperSet> UserSuperSets { get; set; }
    public virtual ICollection<UserExercise> UserExercises { get; set; }
    public virtual ICollection<UserLastTraining> UserLastTrainings { get; set; }
    public virtual ICollection<UserLastTrainingExercise> UserLastTrainingExercises { get; set; }
    public virtual ICollection<UserTraining> UserTrainings { get; set; }
    public virtual ICollection<UserTrainingExercise> UserTrainingExercises { get; set; }
    public virtual ICollection<UserTrainingGroup> UserTrainingGroups { get; set; }
    public virtual ICollection<UserWeightNote> UserWeightNotes { get; set; }
    public virtual ICollection<SocialWorkout> SocialWorkouts { get; set; }
    public virtual ICollection<SocialWorkoutLike> SocialWorkoutLikes { get; set; }
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
}
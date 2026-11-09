using TrainingDay.Web.Data.Results;

namespace TrainingDay.Web.Services.SocialWorkouts;

public static class SocialWorkoutErrors
{
    public static readonly Error NotFound = Error.NotFound("SocialWorkouts.NotFound", "Workout not found.");

    public static readonly Error LikeNotFound = Error.NotFound("SocialWorkouts.LikeNotFound", "The workout is not liked.");

    public static readonly Error UserNotFound = Error.NotFound("SocialWorkouts.UserNotFound", "User not found.");

    public static readonly Error SharingDisabled = Error.Forbidden(
        "SocialWorkouts.SharingDisabled", "Sharing completed workouts is disabled in user settings.");
}

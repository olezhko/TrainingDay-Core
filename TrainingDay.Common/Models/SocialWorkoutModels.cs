namespace TrainingDay.Common.Models;

public class SocialWorkoutBaseExerciseDto
{
    public int CodeNum { get; set; }
    public required string WeightAndRepsString { get; set; }
}

public class SocialWorkoutExerciseDto
{
    public required string ExerciseName { get; set; }
    public required IEnumerable<string> MusclesString { get; set; }
    public required string WeightAndRepsString { get; set; }
    public IEnumerable<string> TagsValue { get; set; } = [];
}

public class SocialWorkoutDto
{
    public required string Id { get; set; }
    public required string OwnerUserId { get; set; }
    public required string OwnerNickname { get; set; }
    public required string WorkoutName { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan Duration { get; set; }
    public List<SocialWorkoutBaseExerciseDto> BaseExercises { get; set; } = new();
    public List<SocialWorkoutExerciseDto> CustomExercises { get; set; } = new();
    public int LikesCount { get; set; }
    public bool LikedByMe { get; set; }
}

public class ShareSocialWorkoutRequest
{
    public required string WorkoutName { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan Duration { get; set; }
    public List<SocialWorkoutBaseExerciseDto> BaseExercises { get; set; } = new();
    public List<SocialWorkoutExerciseDto> CustomExercises { get; set; } = new();
}

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

public class UserSettingsDto
{
    public required string Nickname { get; set; }
    public bool ShareCompletedWorkouts { get; set; }
}

public class UpdateUserSettingsRequest
{
    public required string Nickname { get; set; }
    public bool ShareCompletedWorkouts { get; set; }
}

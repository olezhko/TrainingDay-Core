using TrainingDay.Common.Models;
using TrainingDay.Web.Data.Results;

namespace TrainingDay.Web.Services.SocialWorkouts;

public interface ISocialWorkoutManager
{
    /// <summary>Shares as <paramref name="userId"/>, or as the shared anonymous user when null.
    /// Fails with <see cref="SocialWorkoutErrors.UserNotFound"/> or <see cref="SocialWorkoutErrors.SharingDisabled"/>.</summary>
    Task<Result<SocialWorkoutDto>> ShareAsync(Guid? userId, ShareSocialWorkoutRequest request, CancellationToken token);

    Task<PagedResult<SocialWorkoutDto>> GetFeedAsync(Guid? userId, int page, int pageSize, CancellationToken token);

    /// <summary>Liking twice is a no-op. Fails with <see cref="SocialWorkoutErrors.NotFound"/>.</summary>
    Task<Result> LikeAsync(Guid userId, int socialWorkoutId, CancellationToken token);

    /// <summary>Fails with <see cref="SocialWorkoutErrors.LikeNotFound"/>.</summary>
    Task<Result> UnlikeAsync(Guid userId, int socialWorkoutId, CancellationToken token);
}

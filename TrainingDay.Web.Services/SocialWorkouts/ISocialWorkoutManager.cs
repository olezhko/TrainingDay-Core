using TrainingDay.Common.Models;

namespace TrainingDay.Web.Services.SocialWorkouts;

public interface ISocialWorkoutManager
{
    Task<SocialWorkoutDto> ShareAsync(Guid userId, ShareSocialWorkoutRequest request, CancellationToken token);
    Task<PagedResult<SocialWorkoutDto>> GetFeedAsync(Guid? userId, int page, int pageSize, CancellationToken token);
    Task<bool> LikeAsync(Guid userId, int socialWorkoutId, CancellationToken token);
    Task<bool> UnlikeAsync(Guid userId, int socialWorkoutId, CancellationToken token);
}

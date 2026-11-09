using TrainingDay.Common.Communication;

namespace TrainingDay.Web.Services.YoutubeVideo;

public interface IYoutubeVideoManager
{
    /// <summary>
    /// Videos for an exercise, served from the database cache and refreshed from YouTube when older than 30 days.
    /// Returns an empty list when YouTube has nothing for it.
    /// </summary>
    Task<IReadOnlyList<YoutubeVideoItem>> GetVideosAsync(string exerciseName, CancellationToken token);
}

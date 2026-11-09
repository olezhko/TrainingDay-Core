using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TrainingDay.Common.Communication;
using TrainingDay.Web.Database;
using TrainingDay.Web.Entities;

namespace TrainingDay.Web.Services.YoutubeVideo;

public class YoutubeVideoManager(TrainingDayContext context, IYoutubeVideoCatalog videoCatalog) : IYoutubeVideoManager
{
    private static readonly TimeSpan CacheLifetime = TimeSpan.FromDays(30);

    public async Task<IReadOnlyList<YoutubeVideoItem>> GetVideosAsync(string exerciseName, CancellationToken token)
    {
        var cached = await context.ExerciseVideoLinks.FirstOrDefaultAsync(m => m.ExerciseName == exerciseName, token);
        if (cached != null && DateTime.UtcNow - cached.UpdatedDateTime <= CacheLifetime)
        {
            return JsonConvert.DeserializeObject<List<YoutubeVideoItem>>(cached.VideoUrlList) ?? [];
        }

        var items = await videoCatalog.GetVideoItemsAsync(exerciseName, token: token);
        if (items.Count == 0)
        {
            return items;
        }

        if (cached == null)
        {
            context.ExerciseVideoLinks.Add(new ExerciseVideoLink
            {
                ExerciseName = exerciseName,
                UpdatedDateTime = DateTime.UtcNow,
                VideoUrlList = JsonConvert.SerializeObject(items)
            });
        }
        else
        {
            cached.VideoUrlList = JsonConvert.SerializeObject(items);
            cached.UpdatedDateTime = DateTime.UtcNow;
        }

        await context.SaveChangesAsync(token);
        return items;
    }
}

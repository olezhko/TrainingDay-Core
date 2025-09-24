using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TrainingDay.Common.Communication;
using TrainingDay.Web.Database;
using TrainingDay.Web.Entities;
using TrainingDay.Web.Services.YoutubeVideo;

namespace TrainingDay.Web.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class YouTubeVideosController(TrainingDayContext context, IYoutubeVideoCatalog videoCatalog) : ControllerBase
    {
        [HttpGet("{name}")]
        public async Task<ActionResult<IEnumerable<YoutubeVideoItem>>> GetYouTubeVideos([FromRoute] string name)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                name = name.Replace('+', ' ');
                ExerciseVideoLink model = await context.ExerciseVideoLinks.FirstOrDefaultAsync(m => m.ExerciseName == name);

                if (model == null)
                {
                    var items = videoCatalog.GetVideoItemsAsync(name).Result;
                    if (items.Count == 0)
                    {
                        return NoContent();
                    }

                    model = new ExerciseVideoLink
                    {
                        ExerciseName = name,
                        UpdatedDateTime = DateTime.UtcNow,
                        VideoUrlList = JsonConvert.SerializeObject(items)
                    };

                    context.ExerciseVideoLinks.Add(model);
                    await context.SaveChangesAsync();
                    return items;
                }

                if (DateTime.UtcNow - model.UpdatedDateTime > TimeSpan.FromDays(30))
                {
                    var items = videoCatalog.GetVideoItemsAsync(name).Result;
                    if (items.Count == 0)
                    {
                        return NoContent();
                    }
                    model.VideoUrlList = JsonConvert.SerializeObject(items);
                    model.UpdatedDateTime = DateTime.UtcNow;
                    context.Update(model);
                    await context.SaveChangesAsync();
                    return items;
                }

                var result = JsonConvert.DeserializeObject<List<YoutubeVideoItem>>(model.VideoUrlList);
                return result;
            }
            catch (Exception e)
            {
                return BadRequest(new Tuple<string, Exception>(name, e));
            }
        }
    }
}

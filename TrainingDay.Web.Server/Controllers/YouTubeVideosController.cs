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
    public class YouTubeVideosController : ControllerBase
    {
        private readonly TrainingDayContext _context;
        private readonly ILogger<YouTubeVideosController> _logger;
        private readonly IYoutubeVideoCatalog _youtubeVideoCatalog;

        public YouTubeVideosController(TrainingDayContext context, ILogger<YouTubeVideosController> logger,
            IYoutubeVideoCatalog youtubeVideoCatalog)
        {
            _context = context;
            _logger = logger;
            _youtubeVideoCatalog = youtubeVideoCatalog;
        }

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
                ExerciseVideoLink model = await _context.ExerciseVideoLinks.FirstOrDefaultAsync(m => m.ExerciseName == name);

                if (model == null)
                {
                    model = new ExerciseVideoLink { ExerciseName = name, UpdatedDateTime = DateTime.UtcNow };
                    var items = _youtubeVideoCatalog.GetVideoItemsAsync(name).Result;
                    if (items.Count == 0)
                    {
                        return NoContent();
                    }
                    model.VideoUrlList = JsonConvert.SerializeObject(items);
                    _context.ExerciseVideoLinks.Add(model);
                    await _context.SaveChangesAsync();
                    return items;
                }

                if (DateTime.UtcNow - model.UpdatedDateTime > TimeSpan.FromDays(30))
                {
                    var items = _youtubeVideoCatalog.GetVideoItemsAsync(name).Result;
                    if (items.Count == 0)
                    {
                        return NoContent();
                    }
                    model.VideoUrlList = JsonConvert.SerializeObject(items);
                    model.UpdatedDateTime = DateTime.UtcNow;
                    _context.Update(model);
                    await _context.SaveChangesAsync();
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

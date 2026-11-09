using Microsoft.AspNetCore.Mvc;
using TrainingDay.Common.Communication;
using TrainingDay.Web.Services.YoutubeVideo;

namespace TrainingDay.Web.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class YouTubeVideosController(IYoutubeVideoManager videoManager) : ControllerBase
    {
        [HttpGet("{name}")]
        public async Task<ActionResult<IEnumerable<YoutubeVideoItem>>> GetYouTubeVideos([FromRoute] string name, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var items = await videoManager.GetVideosAsync(name.Replace('+', ' '), cancellationToken);
            if (items.Count == 0)
            {
                return NoContent();
            }

            return Ok(items);
        }
    }
}

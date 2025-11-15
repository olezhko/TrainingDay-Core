using Microsoft.AspNetCore.Mvc;
using TrainingDay.Common.Communication;
using TrainingDay.Web.Services.Blogs;

namespace TrainingDay.Web.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MobileBlogsController(IBlogPostsManager blogPostsManager) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetMobileBlogsAsync([FromQuery] int? cultureId, DateTime? createdFilter, CancellationToken token)
    {
        var dataPage = await blogPostsManager.GetMobileBlogsAsync(cultureId, createdFilter, token);
        return Ok(dataPage);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetMobileBlogAsync(int id, CancellationToken token)
    {
        var blog = await blogPostsManager.GetAsync(id, token);

        if (blog == null)
        {
            return NotFound();
        }

        var result = new BlogResponse()
        {
            Guid = blog.Id,
            Title = blog.Title,
            Published = blog.Date,
            Content = blog.View
        };

        return Ok(result);
    }
}
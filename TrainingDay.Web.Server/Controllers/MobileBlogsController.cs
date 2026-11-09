using Microsoft.AspNetCore.Mvc;
using TrainingDay.Common.Communication;
using TrainingDay.Web.Server.Extensions;
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
        var result = await blogPostsManager.GetAsync(id, token);
        if (result.IsFailure)
        {
            return this.Problem(result.Error);
        }

        var blog = result.Value;

        var response = new BlogResponse()
        {
            Id = blog.Id,
            Title = blog.Title,
            Published = blog.Date,
            Content = blog.View,
            Tags = blog.Tags
        };

        return Ok(response);
    }
}
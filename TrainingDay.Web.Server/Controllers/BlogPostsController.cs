using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrainingDay.Web.Data.BlogPosts;
using TrainingDay.Web.Server.Extensions;
using TrainingDay.Web.Services.Blogs;

namespace TrainingDay.Web.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BlogPostsController(IBlogPostsManager blogService) : ControllerBase
{
    [HttpGet("search")]
    public async Task<IActionResult> GetAsync(BlogsCultureTypes culture, int page, int pageSize, CancellationToken token)
    {
        return Ok(await blogService.Get(culture, page, pageSize, token));
    }

    [HttpGet]
    public async Task<IActionResult> GetAsync(int id, CancellationToken token)
    {
        var result = await blogService.GetAsync(id, token);
        if (result.IsFailure)
        {
            return this.Problem(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet("editor")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetEditorDataAsync(CancellationToken token)
    {
        return Ok(await blogService.CreateEditorDataAsync(token));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateAsync([FromBody] BlogPostEditViewModel blogPost, CancellationToken token)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await blogService.CreateAsync(blogPost, token);
        if (result.IsFailure)
        {
            return this.Problem(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpPut]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> EditAsync(int id, [FromBody] BlogPostEditViewModel blogPost, CancellationToken token)
    {
        if (id != blogPost.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var result = await blogService.EditAsync(blogPost, token);
        if (result.IsFailure)
        {
            return this.Problem(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpDelete]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteAsync(int id, CancellationToken token)
    {
        var result = await blogService.DeleteAsync(id, token);
        if (result.IsFailure)
        {
            return this.Problem(result.Error);
        }

        return Ok();
    }
}

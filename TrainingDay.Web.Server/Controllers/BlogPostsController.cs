using Microsoft.AspNetCore.Mvc;
using TrainingDay.Web.Data.BlogPosts;
using TrainingDay.Web.Entities;
using TrainingDay.Web.Services.Blogs;

namespace TrainingDay.Web.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogPostsController(ILogger<BlogPostsController> logger, IBlogPostsManager blogService) : ControllerBase
    {
        [HttpGet("search")]
        public async Task<IActionResult> Get(BlogsCultureTypes culture, int page, int pageSize, CancellationToken token)
        {
            return Ok(await blogService.Get(culture, page, pageSize, token));
        }

        [HttpGet]
        public async Task<IActionResult> Get(int id, CancellationToken token)
        {
            var blogPost = await blogService.Get(id, token);
            if (blogPost == null)
            {
                return NotFound();
            }

            return Ok(blogPost);
        }

        [HttpGet("editor")]
        public IActionResult GetEditorData()
        {
            return Ok(blogService.CreateEditorData());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BlogPostEditViewModel blogPost)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var blog = await blogService.Create(blogPost);

            return Ok(blog);
        }

        [HttpPut]
        public async Task<IActionResult> Edit(int id, [FromBody] BlogPostEditViewModel blogPost)
        {
            if (id != blogPost.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var blog = await blogService.Edit(blogPost);

            return Ok(blog);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            if (await blogService.Delete(id))
            {
                return Ok();
            }

            return NotFound();
        }
    }
}

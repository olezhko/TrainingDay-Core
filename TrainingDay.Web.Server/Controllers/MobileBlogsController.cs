using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingDay.Common.Communication;
using TrainingDay.Web.Database;
using TrainingDay.Web.Server.Extensions;

namespace TrainingDay.Web.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MobileBlogsController : ControllerBase
{
    private readonly TrainingDayContext _context;

    public MobileBlogsController(TrainingDayContext context)
    {
        _context = context;
    }

    [HttpGet("blogs")]
    public async Task<IActionResult> GetMobileBlogs([FromQuery] int cultureId, int page, int pageSize, CancellationToken token)
    {
        IEnumerable<BlogResponse> dataPage = await _context.PostCultures
            .Where(post => post.CultureId == cultureId)
            .Include(item => item.BlogPost)
            .AsNoTracking()
            .Page(page, pageSize)
            .Select(item => new BlogResponse()
            {
                Guid = item.Id.ToString(),
                Title = item.BlogPost.Title,
                Published = item.BlogPost.Date,
            })
            .ToListAsync(token);

        return Ok(dataPage);
    }

    [HttpGet]
    public async Task<IActionResult> GetMobileBlog([FromQuery] int id, CancellationToken token)
    {
        var blog = await _context.PostCultures
            .Include(item => item.BlogPost)
            .AsNoTracking()
            .FirstOrDefaultAsync(post => post.Id == id, token);

        if (blog == null)
        {
            return NotFound();
        }

        var result = new BlogResponse()
        {
            Title = blog.BlogPost.Title,
            Published = blog.BlogPost.Date,
            Content = blog.BlogPost.View
        };

        return Ok(result);
    }
}
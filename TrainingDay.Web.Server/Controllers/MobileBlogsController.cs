using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingDay.Common;
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
        IEnumerable<MobileBlog> dataPage = await _context.PostCultures
            .Where(post => post.CultureId == cultureId)
            .Include(item => item.BlogPost)
            .AsNoTracking()
            .Page(page, pageSize)
            .Select(item => new MobileBlog()
            {
                Id = item.Id,
                Title = item.BlogPost.Title,
                DateTime = item.BlogPost.Date,
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

        var result = new MobileBlog()
        {
            Title = blog.BlogPost.Title,
            DateTime = blog.BlogPost.Date,
            Text = blog.BlogPost.View
        };

        return Ok(result);
    }
}
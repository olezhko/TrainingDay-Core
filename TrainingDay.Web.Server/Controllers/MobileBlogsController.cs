using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using TrainingDay.Common;
using TrainingDay.Web.Database;
using TrainingDay.Web.Entities;
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

    [HttpGet]
    public async Task<IEnumerable<MobileBlog>> GetMobileBlogs([FromQuery] int cultureId, int page = 1, int pageSize = 10)
    {
        IEnumerable<MobileBlog> dataPage = _context.PostCultures
            .Where(post => post.CultureId == cultureId)
            .Include(item => item.BlogPost)
            .AsNoTracking()
            .Page(page, pageSize)
            .Select(item => new MobileBlog()
            {
                Title = item.BlogPost.Title,
                DateTime = item.BlogPost.Date,
                ShortText = item.BlogPost.Description,
                Text = item.BlogPost.View
            });

        return dataPage;
    }
}
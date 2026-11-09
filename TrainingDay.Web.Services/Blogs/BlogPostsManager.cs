using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TrainingDay.Common.Communication;
using TrainingDay.Web.Data.BlogPosts;
using TrainingDay.Web.Data.Blogs;
using TrainingDay.Web.Data.Results;
using TrainingDay.Web.Database;
using TrainingDay.Web.Entities;
using TrainingDay.Web.Services.Firebase;
using TrainingDay.Web.Services.Mapping;

namespace TrainingDay.Web.Services.Blogs;

public class BlogPostsManager(TrainingDayContext context, ILogger<BlogPostsManager> logger, IServiceScopeFactory scopeFactory) : IBlogPostsManager
{
    public async Task<EditorData> CreateEditorDataAsync(CancellationToken token)
    {
        var result = new EditorData();

        // Only the tag strings are needed, not the full posts with their content.
        var tagStrings = await context.Posts.AsNoTracking()
            .Where(post => post.TagsString != null)
            .Select(post => post.TagsString)
            .ToListAsync(token);

        result.Tags = tagStrings
            .SelectMany(tags => tags.Split(","))
            .Distinct()
            .Select(tag => new SelectItem<string>(tag, tag))
            .ToList();

        result.Cultures = await context.Cultures.AsNoTracking()
            .Select(item => new SelectItem<int>(item.Name, item.Id))
            .ToListAsync(token);

        return result;
    }

    public async Task<Result<BlogPostEditViewModel>> CreateAsync(BlogPostEditViewModel blogPost, CancellationToken token)
    {
        if (!Enum.IsDefined(typeof(BlogsCultureTypes), blogPost.CultureId))
        {
            return BlogPostErrors.UnknownCulture;
        }

        // The culture row references the new post through its navigation, so one save inserts both.
        var blog = blogPost.ToEntity();
        context.Add(blog);
        await context.SaveChangesAsync(token);

        StartSendBlogNotify(blogPost.Title, ((BlogsCultureTypes)blogPost.CultureId).ToLanguageCode());

        return blog.ToViewModel();
    }

    // Runs after the request has finished, so it needs its own scope: the request's DbContext is disposed by then.
    private void StartSendBlogNotify(string message, string culture)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var scopedContext = scope.ServiceProvider.GetRequiredService<TrainingDayContext>();
                var scopedFirebase = scope.ServiceProvider.GetRequiredService<IFirebaseService>();

                // Plain equality translates to SQL; the MySQL column collation makes it case-insensitive.
                var tokens = await scopedContext.MobileTokens.AsNoTracking()
                    .Where(item => item.Language == culture)
                    .Select(item => item.Token)
                    .ToListAsync();

                foreach (var token in tokens)
                {
                    await scopedFirebase.SendMessage(token, "New Blog", message, PushNotificationItem.BlogType, null);
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to send blog notifications");
            }
        });
    }

    public async Task<Result<BlogPostEditViewModel>> EditAsync(BlogPostEditViewModel blogPost, CancellationToken token)
    {
        // Update() on a missing row would fail on save with a concurrency exception.
        var exists = await context.PostCultures.AnyAsync(
            item => item.Id == blogPost.Id && item.BlogPostId == blogPost.BlogId, token);
        if (!exists)
        {
            return BlogPostErrors.NotFound;
        }

        var blog = blogPost.ToEntity();
        context.Update(blog);
        await context.SaveChangesAsync(token);

        return blog.ToViewModel();
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken token)
    {
        var blogPost = await context.Posts.SingleOrDefaultAsync(m => m.Id == id, token);
        if (blogPost == null)
        {
            return BlogPostErrors.NotFound;
        }

        context.Posts.Remove(blogPost);
        await context.SaveChangesAsync(token);
        return Result.Success();
    }

    public async Task<IEnumerable<BlogPostEditViewModel>> Get(BlogsCultureTypes culture, int page, int pageSize, CancellationToken token)
    {
        var result = await context.PostCultures
            .Where(item => item.CultureId == (int)culture)
            .Include(item => item.BlogPost)
            .AsNoTracking()
            .OrderByDescending(item => item.BlogPost.Date)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(token);

        return result.Select(item => item.ToViewModel());
    }

    public async Task<IReadOnlyList<BlogSitemapItem>> GetSitemapItemsAsync(BlogsCultureTypes culture, CancellationToken token)
    {
        return await context.PostCultures
            .AsNoTracking()
            .Where(item => item.CultureId == (int)culture)
            .OrderByDescending(item => item.BlogPost.Date)
            .Select(item => new BlogSitemapItem(item.Id, item.BlogPost.Date))
            .ToListAsync(token);
    }

    public async Task<Result<BlogPostEditViewModel>> GetAsync(int id, CancellationToken token)
    {
        var blogPost = await context.PostCultures
            .AsNoTracking()
            .Include(item => item.BlogPost)
            .SingleOrDefaultAsync(m => m.Id == id, cancellationToken: token);

        return blogPost is null ? BlogPostErrors.NotFound : blogPost.ToViewModel();
    }

    public async Task<IEnumerable<BlogResponse>> GetMobileBlogsAsync(int? cultureId, DateTime? createdFilter, CancellationToken token)
    {
        var query = context.PostCultures.Include(item => item.BlogPost).AsNoTracking();
        if (createdFilter.HasValue)
        {
            query = query.Where(post => post.BlogPost.Date >= createdFilter.Value);
        }

        if (cultureId.HasValue)
        {
            query = query.Where(post => post.CultureId == cultureId.Value);
        }

        IEnumerable<BlogResponse> dataPage = await query
            .Select(item => new BlogResponse()
            {
                Id = item.Id,
                Title = item.BlogPost.Title,
                Published = item.BlogPost.Date,
                Tags = item.BlogPost.TagsString.Split(",", StringSplitOptions.RemoveEmptyEntries).ToList()
            })
            .ToListAsync(token);

        return dataPage;
    }
}

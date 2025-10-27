using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TrainingDay.Common.Communication;
using TrainingDay.Web.Data.BlogPosts;
using TrainingDay.Web.Data.Blogs;
using TrainingDay.Web.Database;
using TrainingDay.Web.Entities;
using TrainingDay.Web.Services.Extensions;
using TrainingDay.Web.Services.Firebase;

namespace TrainingDay.Web.Services.Blogs
{
    public class BlogPostsManager(TrainingDayContext context, IMapper mapper, ILogger<BlogPostsManager> logger, IFirebaseService firebaseService) : IBlogPostsManager
    {
        public EditorData CreateEditorData()
        {
            var result = new EditorData();

            foreach (var post in context.Posts)
            {
                if (post.TagsString != null)
                {
                    var tags = post.TagsString.Split(",").ToList();
                    foreach (var newTag in tags
                                 .Select(tag => new SelectItem<string>(tag, tag))
                                 .Where(newTag => result.Tags.All(item => item.Value != newTag.Value)))
                    {
                        result.Tags.Add(newTag);
                    }
                }
            }

            result.Cultures = context.Cultures.Select(item => new SelectItem<int>(item.Name, item.Id)).ToList();

            return result;
        }

        public async Task<BlogPost> Create(BlogPostEditViewModel blogPost)
        {
            var blog = mapper.Map<BlogPostCulture>(blogPost);
            context.Add(blog.BlogPost);

            await context.SaveChangesAsync();

            context.Add(new BlogPostCulture()
            {
                BlogPostId = blog.BlogPost.Id,
                CultureId = blog.CultureId
            });

            await context.SaveChangesAsync();

            var culture = await context.Cultures.FindAsync(blog.CultureId);

            StartSendBlogNotify(blogPost.Title, culture.Code);

            return blog.BlogPost;
        }

        private void StartSendBlogNotify(string message, string culture)
        {
            _ = Task.Run(async () =>
            {
                var tokens = context.MobileTokens.AsNoTracking()
                    .Where(item => string.Equals(item.Language, culture, StringComparison.OrdinalIgnoreCase));
                foreach (var tokenItem in tokens)
                {
                    try
                    {
                        var title = "New Blog";
                        await firebaseService.SendMessage(tokenItem.Token, title, message, PushNotificationItem.BlogType, null);
                    }
                    catch (Exception e)
                    {
                        logger.LogError(e.ToString());
                    }
                }
            });
        }

        public async Task<BlogPost> Edit(BlogPostEditViewModel blogPost)
        {
            var blog = mapper.Map<BlogPostCulture>(blogPost);
            context.Update(blog);
            await context.SaveChangesAsync();

            return blog.BlogPost;
        }

        public async Task<bool> Delete(int id)
        {
            var blogPost = await context.Posts.SingleOrDefaultAsync(m => m.Id == id);
            if (blogPost != null)
            {
                context.Posts.Remove(blogPost);
                await context.SaveChangesAsync();
                return true;
            }

            return false;
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

            return mapper.Map<IEnumerable<BlogPostEditViewModel>>(result);
        }

        public async Task<BlogPostEditViewModel> GetAsync(int id, CancellationToken token)
        {
            var blogPost = await context.PostCultures
                .Include(item => item.BlogPost)
                .SingleOrDefaultAsync(m => m.Id == id);

            var result = mapper.Map<BlogPostEditViewModel>(blogPost);
            return result;
        }

        public async Task<IEnumerable<BlogResponse>> GetMobileBlogsAsync(int? cultureId, DateTime? createdFilter, CancellationToken token)
        {
            var query = context.PostCultures.Include(item => item.BlogPost).AsNoTracking().AsQueryable();
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
                    Guid = item.Id,
                    Title = item.BlogPost.Title,
                    Published = item.BlogPost.Date,
                    Labels = item.BlogPost.TagsString.Split(",", StringSplitOptions.RemoveEmptyEntries).ToList()
                })
                .ToListAsync(token);

            return dataPage;
        }
    }
}

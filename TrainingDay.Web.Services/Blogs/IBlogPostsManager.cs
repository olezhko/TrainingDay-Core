using TrainingDay.Common.Communication;
using TrainingDay.Web.Data.BlogPosts;
using TrainingDay.Web.Data.Blogs;
using TrainingDay.Web.Data.Results;

namespace TrainingDay.Web.Services.Blogs;

public interface IBlogPostsManager
{
    Task<IEnumerable<BlogResponse>> GetMobileBlogsAsync(int? cultureId, DateTime? createdFilter, CancellationToken token);
    /// <summary>Fails with <see cref="BlogPostErrors.NotFound"/>.</summary>
    Task<Result<BlogPostEditViewModel>> GetAsync(int id, CancellationToken token);


    Task<EditorData> CreateEditorDataAsync(CancellationToken token);

    /// <summary>Fails with <see cref="BlogPostErrors.UnknownCulture"/>.</summary>
    Task<Result<BlogPostEditViewModel>> CreateAsync(BlogPostEditViewModel blogPost, CancellationToken token);

    /// <summary>Fails with <see cref="BlogPostErrors.NotFound"/>.</summary>
    Task<Result<BlogPostEditViewModel>> EditAsync(BlogPostEditViewModel blogPost, CancellationToken token);

    /// <summary>Fails with <see cref="BlogPostErrors.NotFound"/>.</summary>
    Task<Result> DeleteAsync(int id, CancellationToken token);

    /// <summary>Posts of one culture, newest first.</summary>
    Task<IReadOnlyList<BlogSitemapItem>> GetSitemapItemsAsync(BlogsCultureTypes culture, CancellationToken token);

    Task<IEnumerable<BlogPostEditViewModel>> Get(BlogsCultureTypes culture, int page, int pageSize, CancellationToken token);
}
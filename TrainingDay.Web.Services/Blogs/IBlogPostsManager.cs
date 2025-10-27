using TrainingDay.Common.Communication;
using TrainingDay.Web.Data.BlogPosts;
using TrainingDay.Web.Data.Blogs;
using TrainingDay.Web.Entities;

namespace TrainingDay.Web.Services.Blogs;

public interface IBlogPostsManager
{
    Task<IEnumerable<BlogResponse>> GetMobileBlogsAsync(int? cultureId, DateTime? createdFilter, CancellationToken token);
    Task<BlogPostEditViewModel> GetAsync(int id, CancellationToken token);


    EditorData CreateEditorData();

    Task<BlogPost> Create(BlogPostEditViewModel blogPost);
    Task<BlogPost> Edit(BlogPostEditViewModel blogPost);
    Task<bool> Delete(int id);
    Task<IEnumerable<BlogPostEditViewModel>> Get(BlogsCultureTypes culture, int page, int pageSize, CancellationToken token);
}
using TrainingDay.Web.Data.BlogPosts;
using TrainingDay.Web.Data.Blogs;
using TrainingDay.Web.Entities;

namespace TrainingDay.Web.Services.Blogs;

public interface IBlogPostsManager
{
    EditorData CreateEditorData();

    Task<BlogPost> Create(BlogPostEditViewModel blogPost);
    Task<BlogPost> Edit(BlogPostEditViewModel blogPost);
    Task<bool> Delete(int id);
    Task<IEnumerable<BlogPostEditViewModel>> Get(int cultureId, int page, int pageSize, CancellationToken token);
    Task<BlogPostEditViewModel> Get(int id, CancellationToken token);
}
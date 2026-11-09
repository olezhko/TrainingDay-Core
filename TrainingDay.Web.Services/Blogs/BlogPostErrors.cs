using TrainingDay.Web.Data.Results;

namespace TrainingDay.Web.Services.Blogs;

public static class BlogPostErrors
{
    public static readonly Error NotFound = Error.NotFound("BlogPosts.NotFound", "Blog post not found.");

    public static readonly Error UnknownCulture = Error.Validation("BlogPosts.UnknownCulture", "Unknown culture.");
}

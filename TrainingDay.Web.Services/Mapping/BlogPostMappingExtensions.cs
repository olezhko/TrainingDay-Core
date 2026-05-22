using TrainingDay.Web.Data.BlogPosts;
using TrainingDay.Web.Entities;

namespace TrainingDay.Web.Services.Mapping;

public static class BlogPostMappingExtensions
{
    public static BlogPostEditViewModel ToViewModel(this BlogPostCulture src) => new BlogPostEditViewModel
    {
        Id = src.Id,
        CultureId = src.CultureId,
        BlogId = src.BlogPost.Id,
        Title = src.BlogPost.Title,
        Description = src.BlogPost.Description,
        View = src.BlogPost.View,
        Date = src.BlogPost.Date,
        Author = src.BlogPost.Author,
        Tags = src.BlogPost.TagsString?.Split(',').ToList() ?? [],
    };

    public static BlogPostCulture ToEntity(this BlogPostEditViewModel src) => new BlogPostCulture
    {
        Id = src.Id,
        CultureId = src.CultureId,
        BlogPost = new BlogPost
        {
            Id = src.BlogId,
            Title = src.Title,
            Description = src.Description,
            View = src.View,
            Date = src.Date,
            Author = src.Author,
            TagsString = string.Join(",", src.Tags),
        },
    };
}

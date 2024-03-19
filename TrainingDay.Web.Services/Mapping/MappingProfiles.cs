using AutoMapper;
using System.Reflection.Metadata;
using TrainingDay.Web.Data.BlogPosts;
using TrainingDay.Web.Entities;

namespace TrainingDay.Web.Services.Mapping;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<BlogPostCulture, BlogPostEditViewModel>()
            .ForMember(item => item.CultureId, (src) => src.MapFrom(blog => blog.CultureId))
            .ForMember(item => item.Id, (src) => src.MapFrom(blog => blog.Id))
            .AfterMap((src, dest) => dest.Title = src.BlogPost.Title)
            .AfterMap((src, dest) => dest.Description = src.BlogPost.Description)
            .AfterMap((src, dest) => dest.View = src.BlogPost.View)
            .AfterMap((src, dest) => dest.Date = src.BlogPost.Date)
            .AfterMap((src, dest) => dest.Author = src.BlogPost.Author)
            .AfterMap((src, dest) => dest.Tags = src.BlogPost.TagsString.Split(new char[] { ',' }).ToList())
            .AfterMap((src, dest) => dest.BlogId = src.BlogPost.Id);

        CreateMap<BlogPostEditViewModel, BlogPostCulture>()
            .ForMember(item => item.CultureId, (src) => src.MapFrom(blog => blog.CultureId))
            .ForMember(item => item.Id, (src) => src.MapFrom(blog => blog.Id))
            .AfterMap((src, dest) => dest.BlogPost = new BlogPost())
            .AfterMap((src, dest) => dest.BlogPost.Title = src.Title)
            .AfterMap((src, dest) => dest.BlogPost.Description = src.Description)
            .AfterMap((src, dest) => dest.BlogPost.View = src.View)
            .AfterMap((src, dest) => dest.BlogPost.Date = src.Date)
            .AfterMap((src, dest) => dest.BlogPost.Author = src.Author)
            .AfterMap((src, dest) => dest.BlogPost.Id = src.BlogId)
            .AfterMap((src, dest) => dest.BlogPost.TagsString = string.Join(",", src.Tags));

    }
}
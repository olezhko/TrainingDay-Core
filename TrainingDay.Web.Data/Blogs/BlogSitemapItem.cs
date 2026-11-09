namespace TrainingDay.Web.Data.Blogs;

/// <summary>A blog post entry for the sitemap: its public id and publish date.</summary>
public sealed record BlogSitemapItem(int Id, DateTime Date);

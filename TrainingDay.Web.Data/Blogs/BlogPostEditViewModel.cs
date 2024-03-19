using System.ComponentModel.DataAnnotations;

namespace TrainingDay.Web.Data.BlogPosts;

public class BlogPostEditViewModel
{
    public int Id { get; set; }

    [Required]
    public string Title { get; set; }
    [Required]
    public string Description { get; set; }
    public List<string> Tags { get; set; } = new List<string>();
    [Required]
    public string Author { get; set; }
    [Required]
    public string View { get; set; }

    [Required] public DateTime Date { get; set; }

    public int CultureId { get; set; }
    public int BlogId { get; set; }
}
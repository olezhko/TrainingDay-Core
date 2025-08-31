using System.ComponentModel.DataAnnotations;

namespace TrainingDay.Web.Entities
{
    public class BlogPost
    {
        [Key]
        public int Id { get; set; }

        [Required] 
        public string Title { get; set; }

        [Required] 
        public string Description { get; set; }

        public DateTime Date { get; set; }

        public string Author { get; set; }

        [Required] 
        public string View { get; set; }

        public string TagsString { get; set; }
    }

    public class BlogPostCulture
    {
        [Key] 
        public int Id { get; set; }
        public int BlogPostId { get; set; }
        public int CultureId { get; set; }

        public virtual Culture Culture { get; set; }
        public virtual BlogPost BlogPost { get; set; }
    }

    public class Culture
    {
        [Key]
        public int Id { get; set; }

        [StringLength(20)]
        public string Name { get; set; }
        [StringLength(2)]
        public string Code { get; set; }
    }
}

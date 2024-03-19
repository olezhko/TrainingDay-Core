using System.ComponentModel.DataAnnotations;

namespace TrainingDay.Web.Entities;

public class YoutubeVideoUrls
{
    [Key]
    public int Id { get; set; }
    public string ExerciseName { get; set; }

    public string VideoUrlList { get; set; }
    public DateTime UpdatedDateTime { get; set; }
}
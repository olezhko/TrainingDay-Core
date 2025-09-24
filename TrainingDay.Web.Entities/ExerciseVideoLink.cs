using System.ComponentModel.DataAnnotations;

namespace TrainingDay.Web.Entities;

public class ExerciseVideoLink
{
    [Key]
    public int Id { get; set; }

    public required string ExerciseName { get; set; }

    public required string VideoUrlList { get; set; }

    public DateTime UpdatedDateTime { get; set; }
}
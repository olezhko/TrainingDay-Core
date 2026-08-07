using System.ComponentModel.DataAnnotations;

namespace TrainingDay.Web.Entities.MobileItems;

public class SocialWorkoutExercise
{
    [Key]
    public int Id { get; set; }

    public int SocialWorkoutId { get; set; }
    public int OrderNumber { get; set; }

    public string ExerciseName { get; set; }
    public string MusclesString { get; set; }
    public string WeightAndRepsString { get; set; }
    public int TagsValue { get; set; }
    public int CodeNum { get; set; }

    public virtual SocialWorkout SocialWorkout { get; set; }
}

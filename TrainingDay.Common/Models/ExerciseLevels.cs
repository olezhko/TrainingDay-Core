namespace TrainingDay.Common.Models;

/// <summary>Recommended values of a base exercise for each training level, matched by <see cref="CodeNum"/>.</summary>
public class ExerciseLevels
{
    public int CodeNum { get; set; }
    public ExerciseLevelValues? Beginner { get; set; }
    public ExerciseLevelValues? Medium { get; set; }
    public ExerciseLevelValues? Profi { get; set; }
}

/// <summary>Weight in kg and repetitions per set, or duration in seconds for exercises by time.</summary>
public class ExerciseLevelValues
{
    public double Weight { get; set; }
    public int Repetitions { get; set; }
    public int Seconds { get; set; }
}

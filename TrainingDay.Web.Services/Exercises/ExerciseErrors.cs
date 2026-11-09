using TrainingDay.Web.Data.Results;

namespace TrainingDay.Web.Services.Exercises;

public static class ExerciseErrors
{
    public static readonly Error NotFound = Error.NotFound("Exercises.NotFound", "Exercise not found.");

    public static readonly Error UnknownCulture = Error.Validation("Exercises.UnknownCulture", "Unknown culture.");

    public static readonly Error ImageTooLarge = Error.Validation("Exercises.ImageTooLarge", "The file is too large.");
}

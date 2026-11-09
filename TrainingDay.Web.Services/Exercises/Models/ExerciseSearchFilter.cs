using TrainingDay.Common.Models;

namespace TrainingDay.Web.Services.Exercises.Models;

/// <summary>Filters for the exercise catalog search. Empty collections mean "no filter".</summary>
public sealed record ExerciseSearchFilter(
    string TwoLetterCulture,
    string? Name,
    IReadOnlyCollection<MusclesEnum> Muscles,
    IReadOnlyCollection<ExerciseTags> Tags,
    IReadOnlyCollection<DifficultTypes> Difficulties);

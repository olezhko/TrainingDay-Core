using TrainingDay.Common.Models;

namespace TrainingDay.Web.Services.Exercises.Models;

/// <summary>An exercise of one culture, with the description, muscles and tags unpacked from their database columns.</summary>
public sealed record ExerciseModel
{
    public int Id { get; init; }

    public required int CodeNum { get; init; }

    public required string Name { get; init; }

    public string? StartingPosition { get; init; }

    public string? Execution { get; init; }

    public string? Advice { get; init; }

    public IReadOnlyList<MusclesEnum> Muscles { get; init; } = [];

    public IReadOnlyList<ExerciseTags> Tags { get; init; } = [];

    public DifficultTypes DifficultType { get; init; }

    /// <summary>Two-letter culture code, e.g. "en".</summary>
    public string? CultureCode { get; init; }
}

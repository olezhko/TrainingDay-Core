namespace TrainingDay.Web.Server.ViewModels.Exercises;

/// <summary>An exercise picked by the AI search, returned by <c>POST api/exercises/query/extended</c>.</summary>
public sealed record WorkoutExerciseResponse(
    int ExerciseId,
    string Name,
    string Description,
    int Code,
    IReadOnlyCollection<NamedValueResponse> Tags,
    IReadOnlyCollection<NamedValueResponse> Muscles);

/// <summary>An enum value with its name.</summary>
public sealed record NamedValueResponse(int Id, string Name);

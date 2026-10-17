using TrainingDay.Common.Communication;

namespace TrainingDay.Web.Data.OpenAI;

public interface IAiService
{
    Task<ExercisesAiResponse> GetExercisesByQueryAsync(string prompt, CancellationToken token);
}

public record ExercisesAiResponse(IEnumerable<ExerciseQueryResponse> Exercises);

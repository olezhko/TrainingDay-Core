using TrainingDay.Common.Communication;

namespace TrainingDay.Web.Data.OpenAI;

public interface IOpenAIService
{
    Task<ExercisesAiResponse> GetExercisesByQueryAsync(string prompt, CancellationToken token);
}

public record ExercisesAiResponse(IEnumerable<ExerciseQueryResponse> exercises);

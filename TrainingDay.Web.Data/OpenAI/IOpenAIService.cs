using TrainingDay.Common.Communication;

namespace TrainingDay.Web.Data.OpenAI;

public interface IOpenAIService
{
    Task<IEnumerable<ExerciseQueryResponse>> GetExercisesByQueryAsync(string prompt, CancellationToken token);
}
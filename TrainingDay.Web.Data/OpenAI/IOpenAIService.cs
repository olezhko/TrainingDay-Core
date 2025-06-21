namespace TrainingDay.Web.Data.OpenAI;

public interface IOpenAIService
{
    Task<IEnumerable<ExerciseOpenAIResponse>> GetExercisesByQuery(string prompt, CancellationToken token);
}
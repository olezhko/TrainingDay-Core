using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Chat;
using TrainingDay.Common.Communication;
using TrainingDay.Common.Extensions;
using TrainingDay.Common.Models;
using TrainingDay.Web.Data.OpenAI;
using TrainingDay.Web.Services.OpenAI;

namespace TrainingDay.Web.Services.DeepSeek;

/// <summary>
/// DeepSeek exposes an OpenAI-compatible chat completions API but no assistants or vector stores,
/// so the whole exercise KB is sent in the system prompt and the answer is constrained by a JSON schema.
/// </summary>
public class DeepSeekService(IOptions<DeepSeekOptions> options) : IAiService
{
    // Created on first use so a missing key fails only AI calls, not every controller that depends on IAiService.
    private readonly Lazy<OpenAIClient> client = new(() => new OpenAIClient(
        new OpenAIAuthentication(options.Value.Key),
        new OpenAISettings(domain: options.Value.Domain)));

    private readonly Lazy<Task<string>> systemPrompt = new(BuildSystemPromptAsync);

    public async Task<ExercisesAiResponse> GetExercisesByQueryAsync(string query, CancellationToken token)
    {
        var messages = new List<Message>
        {
            new Message(Role.System, await systemPrompt.Value),
            new Message(Role.User, query)
        };

        var request = new ChatRequest(
            messages,
            model: options.Value.Model,
            jsonSchema: ExerciseAiPrompt.JsonSchema,
            temperature: options.Value.Temperature);

        var response = await client.Value.ChatEndpoint.GetCompletionAsync(request, token);
        var content = response.FirstChoice?.Message?.ToString() ?? string.Empty;

        return ExerciseAiPrompt.ExtractJsonResponse(content) ?? new ExercisesAiResponse([]);
    }

    private static async Task<string> BuildSystemPromptAsync()
    {
        var exercises = await ResourceExtension.LoadResourceAsync<BaseExercise>("exercises", "en");
        var kb = string.Join(Environment.NewLine, exercises.Select(ExerciseAiPrompt.FormatExercise));

        return
            $$"""
            You are a fitness assistant that selects exercises exclusively from the Exercise KB listed below.

            {{ExerciseAiPrompt.Rules}}

            The JSON object must have this shape, where {{nameof(ExerciseQueryResponse.Guid)}} is the exercise Id from the KB:
            {"{{nameof(ExercisesAiResponse.Exercises)}}": [{"{{nameof(ExerciseQueryResponse.Guid)}}": 12, "{{nameof(ExerciseQueryResponse.CountOfSets)}}": 4, "{{nameof(ExerciseQueryResponse.CountOfRepsOrTime)}}": "10-12", "{{nameof(ExerciseQueryResponse.WorkingWeight)}}": 20.0}]}

            Exercise KB:
            {{kb}}
            """;
    }
}

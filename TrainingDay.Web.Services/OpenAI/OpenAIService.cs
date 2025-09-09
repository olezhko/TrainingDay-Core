using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OpenAI;
using OpenAI.Assistants;
using OpenAI.Files;
using OpenAI.Threads;
using OpenAI.VectorStores;
using System.Data;
using System.Text;
using TrainingDay.Common.Communication;
using TrainingDay.Common.Extensions;
using TrainingDay.Common.Models;
using TrainingDay.Web.Data.OpenAI;

namespace TrainingDay.Web.Services.OpenAI;

public class OpenAIService(IOptions<OpenAISettings> options) : IOpenAIService
{
    private readonly OpenAIClient client = new OpenAIClient(options.Value.Key);
    string? vectorStoreId = null;
    AssistantResponse? assistant;

    private async Task LoadEmbeddedDbAsync(CancellationToken token)
    {
        var exercises = await ResourceExtension.LoadResource<BaseExercise>("exercises", "en");

        await File.WriteAllLinesAsync("exercises.txt", exercises.Select(FormatExercise));

        var file = await client.FilesEndpoint.UploadFileAsync(
            filePath: "exercises.txt",
            purpose: FilePurpose.Assistants);

        var list = new List<string>
        {
            file.Id
        };
        var createVectorStoreRequest = new CreateVectorStoreRequest("Exercise KB", list);
        var vectorStore = await client.VectorStoresEndpoint.CreateVectorStoreAsync(createVectorStoreRequest);
        vectorStoreId = vectorStore.Id;

        await CreateAssistantRequestAsync(token);
    }

    static string FormatExercise(BaseExercise e) => $"Name: {e.Name} | " +
               $"Guid: {e.CodeNum}";

    public async Task<IEnumerable<ExerciseQueryResponse>> GetExercisesByQueryAsync(string query, CancellationToken token)
    {
        if (vectorStoreId is null)
        {
            await LoadEmbeddedDbAsync(token);
        }

        if (assistant is null)
        {
            await CreateAssistantRequestAsync(token);
        }

        var thread = await client.ThreadsEndpoint.CreateThreadAsync(cancellationToken: token);

        // user asks a question
        var message = new Message(query, Role.User);
        await client.ThreadsEndpoint.CreateMessageAsync(thread.Id, message, cancellationToken: token);

        // run the assistant (it will invoke file_search under the hood)
        RunResponse run = await client.ThreadsEndpoint.CreateRunAsync(
            thread.Id,
            new CreateRunRequest(assistant.Id),
            (IServerSentEvent eventData) => Task.CompletedTask, // Explicitly specify the delegate type
            cancellationToken: token);

        // read the answer
        var messages = await client.ThreadsEndpoint.ListMessagesAsync(thread.Id, cancellationToken: token);

        var result = new List<ExerciseQueryResponse>();
        foreach (var m in messages.Items)
        {
            if (m.Role == Role.Assistant)
            {
                var lines = m.PrintContent().Split('\n', StringSplitOptions.RemoveEmptyEntries);
                StringBuilder sb = new StringBuilder();
                for (int i = 2; i < lines.Length; i++)
                {
                    sb.Append(lines[i]);
                    if (lines[i].Contains(']'))
                    {
                        break;
                    }
                }

                IEnumerable<ExerciseQueryResponse> response = JsonConvert.DeserializeObject<IEnumerable<ExerciseQueryResponse>>(sb.ToString());
                return response;
            }
        }

        return result;
    }

    private async Task CreateAssistantRequestAsync(CancellationToken token)
    {
        string assistantInstructions =
$@"You are a fitness assistant.

Your task:
1. Use the user's answers to generate exercises (at least 20) and related information.
2. Try to find in the Exercise KB Vector Store this exercises by matching Name or similar.
3. Order by popularity of exercise and how its better to do exercises.
4. Return JSON result.

Output format:
Return json format for result with properties: 
{nameof(ExerciseQueryResponse.Guid)} (GUID value of exercise from Exercise KB Vector Store), 
{nameof(ExerciseQueryResponse.CountOfSets)} (numeric value), 
{nameof(ExerciseQueryResponse.CountOfRepsOrTime)} (numeric value), 
{nameof(ExerciseQueryResponse.WorkingWeight)} (numeric value in kilograms, offer based on level and difficulty of exercise).";

        assistant = await client.AssistantsEndpoint.CreateAssistantAsync(
            new CreateAssistantRequest(
                name: "Workout Coach",
                instructions: assistantInstructions,
                model: "gpt-4o-mini",
                responseFormat: ChatResponseFormat.Json,
                tools: new[] { Tool.FileSearch },
                toolResources: new ToolResources(fileSearch: new FileSearchResources(vectorStoreId), null)), 
            cancellationToken: token);
    }
}
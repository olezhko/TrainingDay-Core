using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OpenAI;
using OpenAI.Assistants;
using OpenAI.Files;
using OpenAI.Threads;
using OpenAI.VectorStores;
using System.Data;
using System.Diagnostics;
using System.Text;
using TrainingDay.Common.Communication;
using TrainingDay.Common.Extensions;
using TrainingDay.Common.Models;
using TrainingDay.Web.Data.OpenAI;

namespace TrainingDay.Web.Services.OpenAI;

public class OpenAIService(IOptions<Data.OpenAI.OpenAISettings> options) : IOpenAIService
{
    private readonly OpenAIClient client = new OpenAIClient(options.Value.Key);
    string? vectorStoreId = null;
    AssistantResponse? assistant;

    private async Task LoadEmbeddedDbAsync(CancellationToken token)
    {
        var exercises = await ResourceExtension.LoadResource<BaseExercise>("exercises", "en");
        
        await File.WriteAllLinesAsync("exercises.txt", exercises.Select(FormatExercise));

        // 1. Upload to OpenAI (purpose = "assistants")
        var file = await client.FilesEndpoint.UploadFileAsync(
            filePath: "exercises.txt",
            purpose: FilePurpose.Assistants);

        // 2. Create a new vector store that will hold the file
        var list = new List<string>
        {
            file.Id
        };
        var createVectorStoreRequest = new CreateVectorStoreRequest("Exercise KB", list);
        var vectorStore = await client.VectorStoresEndpoint.CreateVectorStoreAsync(createVectorStoreRequest);
        vectorStoreId = vectorStore.Id;

        await CreateAssistantRequestAsync(token);
    }

    static string FormatExercise(BaseExercise e) => $"Name: {e.ExerciseItemName} | " +
               $"Start: {e.Description.StartPosition} | " +
               $"Execution: {e.Description.Execution} | " +
               $"Advice: {e.Description.Advice} | " +
               $"Muscles: {e.MusclesString} | " +
               $"Guid: {e.CodeNum} | " +
               $"Tags: {e.Tags}.";

    public async Task<IEnumerable<ExerciseQueryResponse>> GetExercisesByQuery(string query, CancellationToken token)
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
        assistant = await client.AssistantsEndpoint.CreateAssistantAsync(new CreateAssistantRequest(name: "Workout Coach",
            instructions: "Use for exercises source only Exercise KB Vector Store, no additional sources. " +
            $"Get exercises that followed these questions and answers. Return json format for result with properties: {nameof(ExerciseQueryResponse.Guid)}, {nameof(ExerciseQueryResponse.CountOfSets)}, {nameof(ExerciseQueryResponse.CountOfRepsOrTime)}, {nameof(ExerciseQueryResponse.WorkingWeight)} (in kilograms, if applicable).",
            model: "gpt-4o-mini",
            responseFormat: ChatResponseFormat.Json,
            tools: new[] { Tool.FileSearch },
            toolResources: new ToolResources(fileSearch: new FileSearchResources(vectorStoreId), null)), cancellationToken: token);
    }
}
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OpenAI;
using OpenAI.Assistants;
using OpenAI.Files;
using OpenAI.Threads;
using OpenAI.VectorStores;
using TrainingDay.Common.Extensions;
using TrainingDay.Common.Models;
using TrainingDay.Web.Data.OpenAI;

namespace TrainingDay.Web.Services.OpenAI;

public class OpenAIService(IOptions<OpenAIOptions> options) : IAiService
{
    // Created on first use so a missing key fails only AI calls, not every controller that depends on IAiService.
    private readonly Lazy<OpenAIClient> client = new(() => new OpenAIClient(options.Value.Key));
    private readonly SemaphoreSlim initLock = new SemaphoreSlim(1, 1);

    private const string IdsFilePath = "openai_resource_ids.json";
    private const string VectorStoreName = "Exercise KB";
    private const string AssistantName = "Workout Coach";
    private const string ModelName = "gpt-4o-mini";

    private string? vectorStoreId;
    private string? assistantId;

    private async Task EnsureInitializedAsync(CancellationToken token)
    {
        if (vectorStoreId != null && assistantId != null)
            return;

        await initLock.WaitAsync(token);
        try
        {
            if (vectorStoreId != null && assistantId != null)
                return;

            if (File.Exists(IdsFilePath))
            {
                var json = await File.ReadAllTextAsync(IdsFilePath, token);
                var ids = JsonConvert.DeserializeAnonymousType(json, new { vectorStoreId = "", assistantId = "" });
                vectorStoreId = ids?.vectorStoreId;
                assistantId = ids?.assistantId;
            }

            if (string.IsNullOrEmpty(vectorStoreId))
            {
                await LoadEmbeddedDbAsync(token);
            }
            else if (string.IsNullOrEmpty(assistantId))
            {
                await CreateAssistantAsync(token);
                await PersistIdsAsync(token);
            }
        }
        finally
        {
            initLock.Release();
        }
    }

    private async Task LoadEmbeddedDbAsync(CancellationToken token)
    {
        var exercises = await ResourceExtension.LoadResourceAsync<BaseExercise>("exercises", "en");
        await File.WriteAllLinesAsync("exercises.txt", exercises.Select(ExerciseAiPrompt.FormatExercise), token);

        var file = await client.Value.FilesEndpoint.UploadFileAsync("exercises.txt", FilePurpose.Assistants, token);

        var vectorStore = await client.Value.VectorStoresEndpoint.CreateVectorStoreAsync(
            new CreateVectorStoreRequest(VectorStoreName, new List<string> { file.Id }));
        vectorStoreId = vectorStore.Id;

        await CreateAssistantAsync(token);
        await PersistIdsAsync(token);
    }

    public async Task<ExercisesAiResponse> GetExercisesByQueryAsync(string query, CancellationToken token)
    {
        await EnsureInitializedAsync(token);

        var thread = await client.Value.ThreadsEndpoint.CreateThreadAsync(cancellationToken: token);
        try
        {
            await client.Value.ThreadsEndpoint.CreateMessageAsync(thread.Id, new Message(query, Role.User), cancellationToken: token);

            await client.Value.ThreadsEndpoint.CreateRunAsync(
                thread.Id,
                new CreateRunRequest(assistantId),
                (IServerSentEvent _) => Task.CompletedTask,
                cancellationToken: token);

            var messages = await client.Value.ThreadsEndpoint.ListMessagesAsync(thread.Id, cancellationToken: token);

            foreach (var m in messages.Items)
            {
                if (m.Role == Role.Assistant)
                {
                    var response = ExerciseAiPrompt.ExtractJsonResponse(m.PrintContent());
                    if (response != null)
                        return response;
                }
            }
        }
        finally
        {
            await client.Value.ThreadsEndpoint.DeleteThreadAsync(thread.Id, CancellationToken.None);
        }

        return new ExercisesAiResponse([]);
    }

    private async Task CreateAssistantAsync(CancellationToken token)
    {
        string instructions =
            $"""
            You are a fitness assistant that selects exercises exclusively from the Exercise KB vector store.

            {ExerciseAiPrompt.Rules}
            """;

        var newAssistant = await client.Value.AssistantsEndpoint.CreateAssistantAsync(
            new CreateAssistantRequest(
                name: AssistantName,
                instructions: instructions,
                model: ModelName,
                jsonSchema: ExerciseAiPrompt.JsonSchema,
                tools: new[] { Tool.FileSearch },
                toolResources: new ToolResources(fileSearch: new FileSearchResources(vectorStoreId), null)),
            cancellationToken: token);

        assistantId = newAssistant.Id;
    }

    private async Task PersistIdsAsync(CancellationToken token)
    {
        var json = JsonConvert.SerializeObject(new { vectorStoreId, assistantId });
        await File.WriteAllTextAsync(IdsFilePath, json, token);
    }
}

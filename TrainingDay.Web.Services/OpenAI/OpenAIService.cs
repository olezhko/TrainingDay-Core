using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OpenAI;
using OpenAI.Assistants;
using OpenAI.Files;
using OpenAI.Threads;
using OpenAI.VectorStores;
using System.Text.RegularExpressions;
using TrainingDay.Common.Communication;
using TrainingDay.Common.Extensions;
using TrainingDay.Common.Models;
using TrainingDay.Web.Data.OpenAI;

namespace TrainingDay.Web.Services.OpenAI;

public class OpenAIService(IOptions<OpenAIOptions> options) : IOpenAIService
{
    private readonly OpenAIClient client = new OpenAIClient(options.Value.Key);
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
        await File.WriteAllLinesAsync("exercises.txt", exercises.Select(FormatExercise), token);

        var file = await client.FilesEndpoint.UploadFileAsync("exercises.txt", FilePurpose.Assistants, token);

        var vectorStore = await client.VectorStoresEndpoint.CreateVectorStoreAsync(
            new CreateVectorStoreRequest(VectorStoreName, new List<string> { file.Id }));
        vectorStoreId = vectorStore.Id;

        await CreateAssistantAsync(token);
        await PersistIdsAsync(token);
    }

    static string FormatExercise(BaseExercise e) =>
        $"Id: {e.CodeNum} | Name: {e.Name} | Muscles: {e.MusclesString} | Equipment: {e.Tags} | Difficulty: {e.DifficultLevel}";

    public async Task<ExercisesAiResponse> GetExercisesByQueryAsync(string query, CancellationToken token)
    {
        await EnsureInitializedAsync(token);

        var thread = await client.ThreadsEndpoint.CreateThreadAsync(cancellationToken: token);
        try
        {
            await client.ThreadsEndpoint.CreateMessageAsync(thread.Id, new Message(query, Role.User), cancellationToken: token);

            await client.ThreadsEndpoint.CreateRunAsync(
                thread.Id,
                new CreateRunRequest(assistantId),
                (IServerSentEvent _) => Task.CompletedTask,
                cancellationToken: token);

            var messages = await client.ThreadsEndpoint.ListMessagesAsync(thread.Id, cancellationToken: token);

            foreach (var m in messages.Items)
            {
                if (m.Role == Role.Assistant)
                {
                    var response = ExtractJsonResponse(m.PrintContent());
                    if (response != null)
                        return response;
                }
            }
        }
        finally
        {
            await client.ThreadsEndpoint.DeleteThreadAsync(thread.Id, CancellationToken.None);
        }

        return new ExercisesAiResponse([]);
    }

    private static ExercisesAiResponse? ExtractJsonResponse(string content)
    {
        var match = Regex.Match(content, @"\{[\s\S]*\}", RegexOptions.Singleline);
        if (!match.Success)
            return null;
        return JsonConvert.DeserializeObject<ExercisesAiResponse>(match.Value);
    }

    private async Task CreateAssistantAsync(CancellationToken token)
    {
        string instructions =
            $"""
            You are a fitness assistant that selects exercises exclusively from the Exercise KB vector store.

            Equipment tags in KB: CanDoAtHome=Bodyweight, BarbellExist=Barbell, DumbbellExist=Dumbbell, BenchExist=Bench.
            Difficulty levels: Easy=1, Medium=2 (Intermediate), Hard=3.
            Muscles field is comma-separated enum values: Chest, Biceps, Triceps, ShouldersFront, ShouldersBack, ShouldersMiddle,
              WidestBack, MiddleBack, Quadriceps, Buttocks, Abdominal, LowerBack, Trapezium, Forearm, Calves, Cardio, etc.
            Supersets: group two or more exercises back-to-back (assign them the same superset group).

            Your task:
            1. Parse the user's query: extract muscles, equipment, difficulty, goals, duration, and exercise types.
            2. Select at least 20 exercises from the KB that strictly match the requested muscles and equipment tags.
            3. Order logically: compound movements first, then isolation, cooldown last.
            4. Set {nameof(ExerciseQueryResponse.CountOfSets)}, {nameof(ExerciseQueryResponse.CountOfRepsOrTime)}, and
               {nameof(ExerciseQueryResponse.WorkingWeight)} (in kg) based on the user's goal and difficulty level.

            Output: return only a raw JSON object — no markdown, no explanation.
            """;

        var newAssistant = await client.AssistantsEndpoint.CreateAssistantAsync(
            new CreateAssistantRequest(
                name: AssistantName,
                instructions: instructions,
                model: ModelName,
                jsonSchema: GetJsonSchema(),
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

    private JsonSchema GetJsonSchema() => new JsonSchema("exercise_response", ((JsonSchema)typeof(ExercisesAiResponse)).Schema);
}

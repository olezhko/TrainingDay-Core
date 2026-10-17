using Newtonsoft.Json;
using OpenAI;
using System.Text.RegularExpressions;
using TrainingDay.Common.Communication;
using TrainingDay.Common.Models;
using TrainingDay.Web.Data.OpenAI;

namespace TrainingDay.Web.Services.OpenAI;

/// <summary>
/// Prompt pieces shared by all AI providers that pick exercises from the exercise knowledge base.
/// </summary>
public static class ExerciseAiPrompt
{
    public static readonly string Rules =
        $"""
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

    public static readonly JsonSchema JsonSchema =
        new JsonSchema("exercise_response", ((JsonSchema)typeof(ExercisesAiResponse)).Schema);

    public static string FormatExercise(BaseExercise e) =>
        $"Id: {e.CodeNum} | Name: {e.Name} | Muscles: {e.MusclesString} | Equipment: {e.Tags} | Difficulty: {e.DifficultLevel}";

    public static ExercisesAiResponse? ExtractJsonResponse(string content)
    {
        var match = Regex.Match(content, @"\{[\s\S]*\}", RegexOptions.Singleline);
        if (!match.Success)
            return null;
        return JsonConvert.DeserializeObject<ExercisesAiResponse>(match.Value);
    }
}

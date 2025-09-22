using Microsoft.EntityFrameworkCore;
using TrainingDay.Common.Extensions;
using TrainingDay.Common.Models;
using TrainingDay.Web.Database;
using TrainingDay.Web.Entities;
using TrainingDay.Web.Services.Exercises.Models;

namespace TrainingDay.Web.Services.Exercises;

public class ExerciseManager(TrainingDayContext context) : IExerciseManager
{
    public WebExercise CreateExercise(int cultureId)
    {
        return new WebExercise()
        {
            CultureId = cultureId,
            CodeNum = GetLastCode(cultureId) + 1,
            TagsValue = ExerciseExtensions.ConvertTagListToInt(
            [
                ExerciseTags.DatabaseExercise
            ]),
        };
    }

    public async Task<IReadOnlyCollection<WorkoutExercise>> GetExercisesByCodesAsync(IEnumerable<int> codes, CancellationToken token)
    {
        var exercises = await context.Exercises
            .AsNoTracking()
            .Where(item => codes.Contains(item.CodeNum))
            .ToListAsync();

        IReadOnlyCollection<WorkoutExercise> result = [.. exercises.Select(item => item.ToDomain())];

        return result;
    }

    public int GetLastCode(int cultureId)
    {
        return context.Exercises.AsNoTracking()
            .Where(item => item.CultureId == cultureId)
            .Select(item => item.CodeNum)
            .Max();
    }
}
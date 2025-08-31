using Microsoft.EntityFrameworkCore;
using TrainingDay.Common.Extensions;
using TrainingDay.Common.Models;
using TrainingDay.Web.Database;
using TrainingDay.Web.Entities;

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

    public int GetLastCode(int cultureId)
    {
        return context.Exercises.AsNoTracking()
            .Where(item => item.CultureId == cultureId)
            .Select(item => item.CodeNum)
            .Max();
    }
}
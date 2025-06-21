using Microsoft.EntityFrameworkCore;
using TrainingDay.Common.Extensions;
using TrainingDay.Common.Models;
using TrainingDay.Web.Database;
using TrainingDay.Web.Entities;

namespace TrainingDay.Web.Services.Exercises;

public class ExerciseManager(TrainingDayContext context) : IExerciseManager
{
    public WebExercise CreateExercise(string culture)
    {
        return new WebExercise()
        {
            Culture = culture,
            CodeNum = GetLastCode(culture) + 1,
            TagsValue = ExerciseExtensions.ConvertTagListToInt(
            [
                ExerciseTags.DatabaseExercise
            ]),
        };
    }

    public IEnumerable<WebExercise> GetExercises()
    {
        return context.Exercises.AsNoTracking()
            .Where(item => item.Culture == "en");
    }

    public int GetLastCode(string cu)
    {
        return context.Exercises.AsNoTracking()
            .Where(item => item.Culture == cu)
            .Select(item => item.CodeNum)
            .Max();
    }
}
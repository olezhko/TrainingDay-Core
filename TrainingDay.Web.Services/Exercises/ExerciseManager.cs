using Microsoft.EntityFrameworkCore;
using TrainingDay.Common;
using TrainingDay.Web.Database;
using WebExercise = TrainingDay.Web.Entities.WebExercise;

namespace TrainingDay.Web.Services.Exercises;

public class ExerciseManager : IExerciseManager
{
    private TrainingDayContext _context;
    public ExerciseManager(TrainingDayContext context)
    {
        _context = context;
    }

    public WebExercise CreateExercise(string culture)
    {
        return new WebExercise()
        {
            Culture = culture,
            CodeNum = GetLastCode(culture) + 1,
            TagsValue = ExerciseTools.ConvertTagListToInt(
            [
                ExerciseTags.DatabaseExercise
            ]),
        };
    }

    public int GetLastCode(string cu)
    {
        return _context.Exercises.AsNoTracking()
            .Where(item => item.Culture == cu)
            .Select(item => item.CodeNum)
            .Max();
    }
}
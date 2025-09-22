using TrainingDay.Web.Entities;
using TrainingDay.Web.Services.Exercises.Models;

namespace TrainingDay.Web.Services.Exercises;

public interface IExerciseManager
{
    WebExercise CreateExercise(int cultureId);

    Task<IReadOnlyCollection<WorkoutExercise>> GetExercisesByCodesAsync(IEnumerable<int> codes, CancellationToken token);

    int GetLastCode(int cultureId);
}
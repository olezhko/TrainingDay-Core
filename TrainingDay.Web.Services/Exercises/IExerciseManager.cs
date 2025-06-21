using TrainingDay.Web.Entities;

namespace TrainingDay.Web.Services.Exercises;

public interface IExerciseManager
{
    WebExercise CreateExercise(string cu);
    IEnumerable<WebExercise> GetExercises();
    int GetLastCode(string cu);
}
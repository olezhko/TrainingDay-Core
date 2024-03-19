using WebExercise = TrainingDay.Web.Entities.WebExercise;

namespace TrainingDay.Web.Services.Exercises;

public interface IExerciseManager
{
    WebExercise CreateExercise(string cu);
    int GetLastCode(string cu);
}
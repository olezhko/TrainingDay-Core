using TrainingDay.Web.Entities;

namespace TrainingDay.Web.Services.Exercises;

public interface IExerciseManager
{
    WebExercise CreateExercise(int cultureId);
    int GetLastCode(int cultureId);
}
using TrainingDay.Web.Data.Results;
using TrainingDay.Web.Services.Exercises.Models;

namespace TrainingDay.Web.Services.Exercises;

public interface IExerciseManager
{
    Task<IReadOnlyCollection<WorkoutExercise>> GetExercisesByCodesAsync(IEnumerable<int> codes, CancellationToken token);

    /// <summary>Fails with <see cref="ExerciseErrors.UnknownCulture"/>.</summary>
    Task<Result<int>> GetLastCodeAsync(string cultureCode, CancellationToken token);

    /// <summary>Exercises of one culture, ordered by code.</summary>
    Task<IReadOnlyList<ExerciseModel>> SearchAsync(ExerciseSearchFilter filter, CancellationToken token);

    /// <summary>Fails with <see cref="ExerciseErrors.NotFound"/>.</summary>
    Task<Result<ExerciseModel>> GetByCodeAsync(int codeNum, string cultureCode, CancellationToken token);

    Task<IReadOnlyList<int>> GetCodesAsync(string cultureCode, CancellationToken token);

    /// <summary>Adds the exercise to <see cref="ExerciseModel.CultureCode"/>. Fails with <see cref="ExerciseErrors.UnknownCulture"/>.</summary>
    Task<Result<ExerciseModel>> CreateAsync(ExerciseModel exercise, CancellationToken token);

    /// <summary>Copies the editable fields onto the stored exercise. Fails with <see cref="ExerciseErrors.NotFound"/>.</summary>
    Task<Result<ExerciseModel>> UpdateAsync(int id, ExerciseModel changes, CancellationToken token);

    /// <summary>Fails with <see cref="ExerciseErrors.NotFound"/>.</summary>
    Task<Result> DeleteAsync(int id, CancellationToken token);

    /// <summary>
    /// Saves the image as <c>{codeNum}.jpg</c> in <paramref name="imagesFolder"/>, replacing any existing one.
    /// Fails with <see cref="ExerciseErrors.ImageTooLarge"/> when <paramref name="length"/> is 2 MB or more.
    /// </summary>
    Task<Result> SaveImageAsync(int codeNum, Stream content, long length, string imagesFolder, CancellationToken token);
}

using Microsoft.EntityFrameworkCore;
using TrainingDay.Web.Data.Results;
using TrainingDay.Web.Database;
using TrainingDay.Web.Entities;
using TrainingDay.Web.Services.Exercises.Models;
using TrainingDay.Web.Services.Mapping;

namespace TrainingDay.Web.Services.Exercises;

public class ExerciseManager(TrainingDayContext context) : IExerciseManager
{
    private const long MaxImageBytes = 2 * 1024 * 1024;

    public async Task<IReadOnlyCollection<WorkoutExercise>> GetExercisesByCodesAsync(IEnumerable<int> codes, CancellationToken token)
    {
        var exercises = await context.Exercises
            .AsNoTracking()
            .Where(item => codes.Contains(item.CodeNum))
            .ToListAsync(token);

        IReadOnlyCollection<WorkoutExercise> result = [.. exercises.Select(item => item.ToDomain())];

        return result;
    }

    public async Task<Result<int>> GetLastCodeAsync(string cultureCode, CancellationToken token)
    {
        var cultureId = await FindCultureIdAsync(cultureCode, token);
        if (cultureId == null)
        {
            return ExerciseErrors.UnknownCulture;
        }

        // Nullable Max returns null instead of throwing when the culture has no exercises yet.
        return await context.Exercises
            .Where(item => item.CultureId == cultureId.Value)
            .MaxAsync(item => (int?)item.CodeNum, token) ?? 0;
    }

    public async Task<IReadOnlyList<ExerciseModel>> SearchAsync(ExerciseSearchFilter filter, CancellationToken token)
    {
        var query = context.Exercises
            .Include(item => item.Culture)
            .AsNoTracking()
            .Where(item => item.Culture.Code == filter.TwoLetterCulture);

        if (!string.IsNullOrEmpty(filter.Name))
        {
            // The MySQL column collation makes this case-insensitive.
            query = query.Where(item => item.Name.Contains(filter.Name));
        }

        var exercises = await query.OrderBy(item => item.CodeNum).ToListAsync(token);

        // Muscles and tags are packed into one column each, so these filters run on the loaded culture list.
        IEnumerable<ExerciseModel> result = exercises.Select(item => item.ToModel());

        if (filter.Muscles.Count > 0)
        {
            result = result.Where(item => item.Muscles.Any(filter.Muscles.Contains));
        }

        if (filter.Tags.Count > 0)
        {
            result = result.Where(item => item.Tags.Any(filter.Tags.Contains));
        }

        if (filter.Difficulties.Count > 0)
        {
            result = result.Where(item => filter.Difficulties.Contains(item.DifficultType));
        }

        return result.ToList();
    }

    public async Task<Result<ExerciseModel>> GetByCodeAsync(int codeNum, string cultureCode, CancellationToken token)
    {
        var exercise = await context.Exercises
            .Include(item => item.Culture)
            .AsNoTracking()
            .SingleOrDefaultAsync(item => item.CodeNum == codeNum && item.Culture.Code == cultureCode, token);

        return exercise is null ? ExerciseErrors.NotFound : exercise.ToModel();
    }

    public async Task<IReadOnlyList<int>> GetCodesAsync(string cultureCode, CancellationToken token)
    {
        return await context.Exercises
            .Where(item => item.Culture.Code == cultureCode)
            .OrderBy(item => item.CodeNum)
            .Select(item => item.CodeNum)
            .ToListAsync(token);
    }

    public async Task<Result<ExerciseModel>> CreateAsync(ExerciseModel exercise, CancellationToken token)
    {
        var cultureId = await FindCultureIdAsync(exercise.CultureCode, token);
        if (cultureId == null)
        {
            return ExerciseErrors.UnknownCulture;
        }

        var entity = new WebExercise { CultureId = cultureId.Value };
        exercise.ApplyTo(entity);
        context.Exercises.Add(entity);
        await context.SaveChangesAsync(token);

        await context.Entry(entity).Reference(item => item.Culture).LoadAsync(token);
        return entity.ToModel();
    }

    public async Task<Result<ExerciseModel>> UpdateAsync(int id, ExerciseModel changes, CancellationToken token)
    {
        var existing = await context.Exercises.FindAsync([id], token);
        if (existing == null)
        {
            return ExerciseErrors.NotFound;
        }

        changes.ApplyTo(existing);
        await context.SaveChangesAsync(token);

        await context.Entry(existing).Reference(item => item.Culture).LoadAsync(token);
        return existing.ToModel();
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken token)
    {
        var exercise = await context.Exercises.FindAsync([id], token);
        if (exercise == null)
        {
            return ExerciseErrors.NotFound;
        }

        context.Exercises.Remove(exercise);
        await context.SaveChangesAsync(token);
        return Result.Success();
    }

    public async Task<Result> SaveImageAsync(int codeNum, Stream content, long length, string imagesFolder, CancellationToken token)
    {
        if (length >= MaxImageBytes)
        {
            return ExerciseErrors.ImageTooLarge;
        }

        var filePath = Path.Combine(imagesFolder, codeNum + ".jpg");
        await using var fileStream = new FileStream(filePath, FileMode.Create);
        await content.CopyToAsync(fileStream, token);
        return Result.Success();
    }

    private async Task<int?> FindCultureIdAsync(string? cultureCode, CancellationToken token)
    {
        if (string.IsNullOrEmpty(cultureCode))
        {
            return null;
        }

        return await context.Cultures
            .Where(item => item.Code == cultureCode)
            .Select(item => (int?)item.Id)
            .FirstOrDefaultAsync(token);
    }
}

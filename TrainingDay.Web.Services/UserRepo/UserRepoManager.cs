using Microsoft.EntityFrameworkCore;
using TrainingDay.Common.Models;
using TrainingDay.Web.Database;
using TrainingDay.Web.Entities.MobileItems;
using TrainingDay.Web.Services.Mapping;

namespace TrainingDay.Web.Services.UserRepo;

public class UserRepoManager(TrainingDayContext context) : IUserRepoManager
{
    public async Task<RepositoryBase> GetAsync(Guid userId, CancellationToken token)
    {
        var exercises = await GetForUserAsync(context.UserExercises, userId, token);
        var weights = await GetForUserAsync(context.UserWeightNotes, userId, token);
        var trainingExercises = await GetForUserAsync(context.UserTrainingExercises, userId, token);
        var trainings = await GetForUserAsync(context.UserTrainings, userId, token);
        var superSets = await GetForUserAsync(context.UserSuperSets, userId, token);
        var lastTrainingExercises = await GetForUserAsync(context.UserLastTrainingExercises, userId, token);
        var lastTrainings = await GetForUserAsync(context.UserLastTrainings, userId, token);
        var trainingGroups = await GetForUserAsync(context.UserTrainingGroups, userId, token);

        return new RepositoryBase
        {
            Exercises = exercises.Select(item => item.ToModel()).ToList(),
            BodyControl = weights.Select(item => item.ToModel()).ToList(),
            TrainingExercise = trainingExercises.Select(item => item.ToModel()).ToList(),
            Trainings = trainings.Select(item => item.ToModel()).ToList(),
            SuperSets = superSets.Select(item => item.ToModel()).ToList(),
            LastTrainingExercises = lastTrainingExercises.Select(item => item.ToModel()).ToList(),
            LastTrainings = lastTrainings.Select(item => item.ToModel()).ToList(),
            TrainingUnions = trainingGroups.Select(item => item.ToModel()).ToList(),
        };
    }

    public async Task UpsertAsync(Guid userId, RepositoryBase data, CancellationToken token)
    {
        await ReplaceForUserAsync(context.UserExercises, userId, data.Exercises, item => new UserExercise(item), token);
        await ReplaceForUserAsync(context.UserWeightNotes, userId, data.BodyControl, item => new UserWeightNote(item), token);
        await ReplaceForUserAsync(context.UserTrainingExercises, userId, data.TrainingExercise, item => new UserTrainingExercise(item), token);
        await ReplaceForUserAsync(context.UserTrainings, userId, data.Trainings, item => new UserTraining(item), token);
        await ReplaceForUserAsync(context.UserSuperSets, userId, data.SuperSets, item => new UserSuperSet(item), token);
        await ReplaceForUserAsync(context.UserLastTrainingExercises, userId, data.LastTrainingExercises, item => new UserLastTrainingExercise(item), token);
        await ReplaceForUserAsync(context.UserLastTrainings, userId, data.LastTrainings, item => new UserLastTraining(item), token);
        await ReplaceForUserAsync(context.UserTrainingGroups, userId, data.TrainingUnions, item => new UserTrainingGroup(item), token);

        await context.SaveChangesAsync(token);
    }

    private static Task<List<TEntity>> GetForUserAsync<TEntity>(DbSet<TEntity> set, Guid userId, CancellationToken token)
        where TEntity : class, IUserEntity
        => set.AsNoTracking().Where(item => item.UserId == userId).ToListAsync(token);

    private static async Task ReplaceForUserAsync<TEntity, TModel>(
        DbSet<TEntity> set,
        Guid userId,
        IEnumerable<TModel> items,
        Func<TModel, TEntity> create,
        CancellationToken token)
        where TEntity : class, IUserEntity
    {
        var existing = await set.Where(item => item.UserId == userId).ToListAsync(token);
        set.RemoveRange(existing);

        var incoming = (items ?? []).Select(create).ToList();
        foreach (var entity in incoming)
        {
            entity.UserId = userId;
        }

        await set.AddRangeAsync(incoming, token);
    }
}

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TrainingDay.Common.Extensions;
using TrainingDay.Common.Models;
using TrainingDay.Web.Database;
using TrainingDay.Web.Entities;
using TrainingDay.Web.Entities.MobileItems;

namespace TrainingDay.Web.Services.SocialWorkouts;

public class SocialWorkoutManager(TrainingDayContext context, UserManager<MobileUser> userManager) : ISocialWorkoutManager
{
    public async Task<SocialWorkoutDto> ShareAsync(Guid userId, ShareSocialWorkoutRequest request, CancellationToken token)
    {
        var user = await context.MobileUsers.FirstOrDefaultAsync(item => item.Id == userId, token)
            ?? throw new KeyNotFoundException($"User not found: {userId}");

        if (!user.ShareCompletedWorkouts)
        {
            throw new InvalidOperationException("Sharing completed workouts is disabled in user settings.");
        }

        if (string.IsNullOrWhiteSpace(user.UserName))
        {
            await userManager.SetUserNameAsync(user, GenerateDefaultNickname());
        }

        var workout = new SocialWorkout
        {
            OwnerUserId = user.Id,
            OwnerNickname = user.UserName,
            WorkoutName = request.WorkoutName,
            Date = request.Date,
            Duration = request.Duration,
            Exercises = request.BaseExercises.Select(item => new SocialWorkoutExercise
            {
                ExerciseName = string.Empty,
                MusclesString = string.Empty,
                WeightAndRepsString = item.WeightAndRepsString,
                TagsValue = 0,
                CodeNum = item.CodeNum,
            }).Concat(request.CustomExercises.Select(item => new SocialWorkoutExercise
            {
                ExerciseName = item.ExerciseName,
                MusclesString = string.Join(",", item.MusclesString),
                WeightAndRepsString = item.WeightAndRepsString,
                TagsValue = ExerciseExtensions.ConvertTagListToInt(item.TagsValue.Select(Enum.Parse<ExerciseTags>)),
                CodeNum = 0,
            })).Select((item, index) =>
            {
                item.OrderNumber = index;
                return item;
            }).ToList(),
        };

        context.SocialWorkouts.Add(workout);
        await context.SaveChangesAsync(token);

        return MapToDto(workout, userId);
    }

    public async Task<PagedResult<SocialWorkoutDto>> GetFeedAsync(Guid userId, int page, int pageSize, CancellationToken token)
    {
        var query = context.SocialWorkouts.AsNoTracking();

        var totalCount = await query.CountAsync(token);

        var workouts = await query
            .Include(item => item.Exercises)
            .Include(item => item.Likes)
            .OrderByDescending(item => item.Date)
            .ThenByDescending(item => item.Likes.Count)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(token);

        return new PagedResult<SocialWorkoutDto>
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            Items = workouts.Select(item => MapToDto(item, userId)).ToList(),
        };
    }

    public async Task<bool> LikeAsync(Guid userId, int socialWorkoutId, CancellationToken token)
    {
        var exists = await context.SocialWorkouts.AnyAsync(item => item.Id == socialWorkoutId, token);
        if (!exists)
        {
            return false;
        }

        var alreadyLiked = await context.SocialWorkoutLikes
            .AnyAsync(item => item.SocialWorkoutId == socialWorkoutId && item.UserId == userId, token);

        if (!alreadyLiked)
        {
            context.SocialWorkoutLikes.Add(new SocialWorkoutLike
            {
                SocialWorkoutId = socialWorkoutId,
                UserId = userId,
                Created = DateTime.UtcNow,
            });
            await context.SaveChangesAsync(token);
        }

        return true;
    }

    public async Task<bool> UnlikeAsync(Guid userId, int socialWorkoutId, CancellationToken token)
    {
        var like = await context.SocialWorkoutLikes
            .FirstOrDefaultAsync(item => item.SocialWorkoutId == socialWorkoutId && item.UserId == userId, token);

        if (like == null)
        {
            return false;
        }

        context.SocialWorkoutLikes.Remove(like);
        await context.SaveChangesAsync(token);
        return true;
    }

    private static string GenerateDefaultNickname() => $"User_{Random.Shared.Next(0, 10001)}";

    private static SocialWorkoutDto MapToDto(SocialWorkout workout, Guid userId) => new()
    {
        Id = workout.Id.ToString(),
        OwnerUserId = workout.OwnerUserId.ToString(),
        OwnerNickname = workout.OwnerNickname,
        WorkoutName = workout.WorkoutName,
        Date = workout.Date,
        Duration = workout.Duration,
        LikesCount = workout.Likes?.Count ?? 0,
        LikedByMe = workout.Likes?.Any(item => item.UserId == userId) ?? false,
        BaseExercises = workout.Exercises
            .Where(item => item.CodeNum > 0)
            .OrderBy(item => item.OrderNumber)
            .Select(item => new SocialWorkoutBaseExerciseDto
            {
                CodeNum = item.CodeNum,
                WeightAndRepsString = item.WeightAndRepsString,
            }).ToList(),
        CustomExercises = workout.Exercises
            .Where(item => item.CodeNum <= 0)
            .OrderBy(item => item.OrderNumber)
            .Select(item => new SocialWorkoutExerciseDto
            {
                ExerciseName = item.ExerciseName,
                MusclesString = ExerciseExtensions.ConvertMuscleStringToList(item.MusclesString).Select(muscle => muscle.ToString()),
                WeightAndRepsString = item.WeightAndRepsString,
                TagsValue = ExerciseExtensions.ConvertTagIntToList(item.TagsValue).Select(tag => tag.ToString()),
            }).ToList(),
    };
}

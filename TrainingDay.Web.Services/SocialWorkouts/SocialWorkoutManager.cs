using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TrainingDay.Common.Extensions;
using TrainingDay.Common.Models;
using TrainingDay.Web.Data.Results;
using TrainingDay.Web.Database;
using TrainingDay.Web.Entities;
using TrainingDay.Web.Entities.MobileItems;

namespace TrainingDay.Web.Services.SocialWorkouts;

public class SocialWorkoutManager(TrainingDayContext context, UserManager<MobileUser> userManager) : ISocialWorkoutManager
{
    public async Task<Result<SocialWorkoutDto>> ShareAsync(Guid? userId, ShareSocialWorkoutRequest request, CancellationToken token)
    {
        var ownerId = userId ?? MobileUser.AnonymousUserId;
        var user = await context.MobileUsers.FirstOrDefaultAsync(item => item.Id == ownerId, token);
        if (user == null)
        {
            return SocialWorkoutErrors.UserNotFound;
        }

        if (!user.ShareCompletedWorkouts)
        {
            return SocialWorkoutErrors.SharingDisabled;
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

        return MapToDto(workout, workout.Exercises, likesCount: 0, likedByMe: false);
    }

    public async Task<PagedResult<SocialWorkoutDto>> GetFeedAsync(Guid? userId, int page, int pageSize, CancellationToken token)
    {
        var totalCount = await context.SocialWorkouts.CountAsync(token);

        // Project likes as a count and a flag instead of including them: a second collection Include
        // multiplies every exercise row by every like.
        var rows = await context.SocialWorkouts.AsNoTracking()
            .OrderByDescending(item => item.Date)
            .ThenByDescending(item => item.Likes.Count)
            .ThenByDescending(item => item.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(item => new
            {
                Workout = item,
                Exercises = item.Exercises.ToList(),
                LikesCount = item.Likes.Count,
                LikedByMe = userId != null && item.Likes.Any(like => like.UserId == userId),
            })
            .ToListAsync(token);

        return new PagedResult<SocialWorkoutDto>
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            Items = rows.Select(item => MapToDto(item.Workout, item.Exercises, item.LikesCount, item.LikedByMe)).ToList(),
        };
    }

    public async Task<Result> LikeAsync(Guid userId, int socialWorkoutId, CancellationToken token)
    {
        var exists = await context.SocialWorkouts.AnyAsync(item => item.Id == socialWorkoutId, token);
        if (!exists)
        {
            return SocialWorkoutErrors.NotFound;
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

        return Result.Success();
    }

    public async Task<Result> UnlikeAsync(Guid userId, int socialWorkoutId, CancellationToken token)
    {
        var like = await context.SocialWorkoutLikes
            .FirstOrDefaultAsync(item => item.SocialWorkoutId == socialWorkoutId && item.UserId == userId, token);

        if (like == null)
        {
            return SocialWorkoutErrors.LikeNotFound;
        }

        context.SocialWorkoutLikes.Remove(like);
        await context.SaveChangesAsync(token);
        return Result.Success();
    }

    private static string GenerateDefaultNickname() => $"User_{Random.Shared.Next(0, 10001)}";

    private static SocialWorkoutDto MapToDto(SocialWorkout workout, IEnumerable<SocialWorkoutExercise> exercises, int likesCount, bool likedByMe) => new()
    {
        Id = workout.Id.ToString(),
        OwnerUserId = workout.OwnerUserId.ToString(),
        OwnerNickname = workout.OwnerNickname,
        WorkoutName = workout.WorkoutName,
        Date = workout.Date,
        Duration = workout.Duration,
        LikesCount = likesCount,
        LikedByMe = likedByMe,
        BaseExercises = exercises
            .Where(item => item.CodeNum > 0)
            .OrderBy(item => item.OrderNumber)
            .Select(item => new SocialWorkoutBaseExerciseDto
            {
                CodeNum = item.CodeNum,
                WeightAndRepsString = item.WeightAndRepsString,
            }).ToList(),
        CustomExercises = exercises
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

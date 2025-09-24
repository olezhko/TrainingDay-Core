using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TrainingDay.Common.Extensions;
using TrainingDay.Common.Models;
using TrainingDay.Web.Entities;

namespace TrainingDay.Web.Database;

public static class ExercisesInitializer
{
    public static async Task InitializeAsync(TrainingDayContext context)
    {
        var itemsToInsert = new List<WebExercise>();

        var cultures = await context.Cultures.AsNoTracking().ToListAsync();
        var exercises = await context.Exercises.AsNoTracking().ToListAsync();
        foreach (var culture in cultures)
        {
            var resourceExercises = await ResourceExtension.LoadResource<BaseExercise>("exercises", culture.Code);
            foreach (var srcExercise in resourceExercises)
            {
                var dbExercise = exercises.FirstOrDefault(item => item.CodeNum == srcExercise.CodeNum && item.CultureId == culture.Id);
                if (dbExercise == null)
                {
                    itemsToInsert.Add(new WebExercise(srcExercise)
                    {
                        CultureId = culture.Id,
                    });
                }
                else
                {
                    dbExercise.DifficultType = srcExercise.DifficultLevel;
                    dbExercise.Description = JsonConvert.SerializeObject(srcExercise.Description);
                    dbExercise.MusclesString = srcExercise.MusclesString;
                    dbExercise.Name = srcExercise.Name;
                }
            }
        }

        if (itemsToInsert.Count > 0)
        {
            await context.Exercises.AddRangeAsync(itemsToInsert);
        }

        await context.SaveChangesAsync();
    }
}
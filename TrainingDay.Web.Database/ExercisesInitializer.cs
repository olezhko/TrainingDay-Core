using Newtonsoft.Json;
using TrainingDay.Common.Extensions;
using TrainingDay.Common.Models;
using TrainingDay.Web.Entities;

namespace TrainingDay.Web.Database;

public static class ExercisesInitializer
{
    public static async Task Initialize(TrainingDayContext context)
    {
        var cultures = new[] { "ru", "en" };
        foreach (var culture in cultures)
        {
            var exercises = await ResourceExtension.LoadResource<BaseExercise>("exercises", culture);
            foreach (var srcExercise in exercises)
            {
                var dbExercise = context.Exercises.FirstOrDefault(item => item.CodeNum == srcExercise.CodeNum && item.Culture == culture);
                if (dbExercise == null)
                {
                    context.Exercises.Add(new WebExercise(srcExercise)
                    {
                        Culture = culture,
                    });
                }
                else
                {
                    dbExercise.Description = JsonConvert.SerializeObject(srcExercise.Description);
                    dbExercise.MusclesString = srcExercise.MusclesString;
                    dbExercise.Name = srcExercise.Name;
                    context.Exercises.Update(dbExercise);
                }
            }
        }

        await context.SaveChangesAsync();
    }
}
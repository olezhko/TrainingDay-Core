using Newtonsoft.Json;
using TrainingDay.Common;
using TrainingDay.Web.Entities;

namespace TrainingDay.Web.Database;

public static class ExercisesInitializer
{
    public static void Initialize(TrainingDayContext context)
    {
        var cultures = new[] { "ru", "en" };
        foreach (var culture in cultures)
        {
            var exercises = ExerciseTools.InitExercises(culture);
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
                    dbExercise.ExerciseItemName = srcExercise.ExerciseItemName;
                    context.Exercises.Update(dbExercise);
                }
            }
        }

        context.SaveChanges();
    }
}
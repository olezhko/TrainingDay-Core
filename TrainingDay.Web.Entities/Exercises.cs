using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace TrainingDay.Web.Entities
{
    public class WebExercise : TrainingDay.Common.Exercise
    {
        [Key]
        public new int Id { get; set; }
        [MaxLength(2)]
        public string Culture { get; set; }
        public WebExercise(TrainingDay.Common.BaseExercise baseExercise)
        {
            Description = JsonConvert.SerializeObject(baseExercise.Description);
            TagsValue = baseExercise.TagsValue;
            CodeNum = baseExercise.CodeNum;
            MusclesString = baseExercise.MusclesString;
            ExerciseItemName = baseExercise.ExerciseItemName;
        }

        public WebExercise()
        {
        }
    }
}

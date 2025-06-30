using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using TrainingDay.Common.Extensions;

namespace TrainingDay.Web.Entities
{
    public class WebExercise : Common.Models.Exercise
    {
        [Key]
        public new int Id { get; set; }
        [MaxLength(2)]
        public string Culture { get; set; }
        public WebExercise(Common.Models.BaseExercise baseExercise)
        {
            Description = JsonConvert.SerializeObject(baseExercise.Description);
            TagsValue = ExerciseExtensions.ConvertTagStringToInt(baseExercise.Tags);
            CodeNum = baseExercise.CodeNum;
            MusclesString = baseExercise.MusclesString;
            Name = baseExercise.Name;
        }

        public WebExercise()
        {
        }
    }
}

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

    public class WebExerciseImageFile
    {
        [Key]
        public int Id { get; set; }
        public int ExerciseCodeNum { get; set; }
        public byte[] Content { get; set; }
    }
}

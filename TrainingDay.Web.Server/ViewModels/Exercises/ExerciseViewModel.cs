using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using TrainingDay.Common;
using WebExercise = TrainingDay.Web.Entities.WebExercise;

namespace TrainingDay.Web.Server.ViewModels.Exercises
{
    public class ExerciseViewModel
    {
        public int Id { get; set; }
        [Required]
        [DisplayName("Name")]
        public string ExerciseItemName { get; set; }

        [DisplayName("Starting Position")]
        public string? StartingPositionDescription { get; set; }

        [DisplayName("Execution")]
        public string? ExecutionDescription { get; set; }

        [DisplayName("Advice")]
        public string? AdviceDescription { get; set; }

        [DisplayName("Muscles")]
        public ObservableCollection<MusclesEnum> Muscles { get; set; }

        [DisplayName("Tags")]
        public List<ExerciseTags> Tags { get; set; }

        [DisplayName("#")]
        [Required]
        public int CodeNum { get; set; }

        [Required]
        public string Culture { get; set; }

        public ExerciseViewModel()
        {
            Muscles = new ObservableCollection<MusclesEnum>();
            Tags = new List<ExerciseTags>();
        }

        public ExerciseViewModel(WebExercise model)
        {
            ExerciseItemName = model.ExerciseItemName;
            CodeNum = model.CodeNum;
            Id = model.Id;

            if (model.Description != null)
            {
                Description descriptionsStrings = JsonConvert.DeserializeObject<Description>(model.Description);
                AdviceDescription = descriptionsStrings.Advice;
                ExecutionDescription = descriptionsStrings.Execution;
                StartingPositionDescription = descriptionsStrings.StartPosition;
            }

            Muscles = new ObservableCollection<MusclesEnum>(ExerciseTools.ConvertMuscleStringToList(model.MusclesString));
            Tags = ExerciseTools.ConvertFromIntToTagList(model.TagsValue);
            Culture = model.Culture;
        }
    }
}

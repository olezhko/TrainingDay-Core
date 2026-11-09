using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using TrainingDay.Common.Models;

namespace TrainingDay.Web.Server.ViewModels.Exercises
{
    public class ExerciseViewModel
    {
        public int Id { get; set; }
        [Required]
        [DisplayName("Name")]
        public string Name { get; set; }

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

        public string? CultureCode { get; set; }

        public DifficultTypes DifficultType { get; set; }

        public ExerciseViewModel()
        {
            Muscles = new ObservableCollection<MusclesEnum>();
            Tags = new List<ExerciseTags>();
        }
    }
}

using Microsoft.AspNetCore.Mvc.Rendering;

namespace TrainingDay.Web.Server.ViewModels.Exercises
{
    public class EditParameters
    {
        public List<SelectListItem> AllTags { get; set; }
        public List<SelectListItem> AllMuscles { get; set; }
        public int OfferedCode { get; set; }
    }
}

using TrainingDay.Common;

namespace TrainingDay.Web.Data.Blogs;

public class EditorData
{
    public List<SelectItem<string>> Tags { get; set; } = new List<SelectItem<string>>();
    public List<SelectItem<int>> Cultures { get; set; } = new List<SelectItem<int>>();
}
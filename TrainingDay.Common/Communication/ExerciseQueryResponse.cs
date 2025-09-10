
namespace TrainingDay.Common.Communication;

public class ExerciseQueryResponse
{
    public int CountOfSets { get; set; }
    public string CountOfRepsOrTime { get; set; }
    public double WorkingWeight { get; set; }
    public int Guid { get; set; }
}
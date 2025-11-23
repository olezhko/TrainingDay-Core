namespace TrainingDay.Common.Communication;

public enum MobileActions
{
    Enter,
    Workout,
    Weight,
}

public class MobileActionDto
{
    public required string Token { get; set; }
    public MobileActions Action { get; set; }
}
namespace TrainingDay.Common.Communication;

public enum MobileActions
{
    Enter,
    Workout,
    Weight,
}

public class MobileAction
{
    public string Token { get; set; }
    public MobileActions Action { get; set; }
    public string Data { get; set; }
}
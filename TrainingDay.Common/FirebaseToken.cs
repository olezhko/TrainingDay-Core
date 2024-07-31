namespace TrainingDay.Common;

public class FirebaseToken
{
    public string Token { get; set; }
    public string Language { get; set; }
    public string Zone { get; set; }
}

public enum MobileActions
{
    Enter,
    Workout,
    Weight
}

public class MobilenAction
{
    public string Token { get; set; }
    public MobileActions Action { get; set; }
}
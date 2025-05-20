namespace TrainingDay.Common.Communication;

/// <summary>
/// Model to send Firebase token to server with user settings
/// </summary>
public class FirebaseToken
{
    public string Token { get; set; }
    public string Language { get; set; }
    public string Zone { get; set; }
}
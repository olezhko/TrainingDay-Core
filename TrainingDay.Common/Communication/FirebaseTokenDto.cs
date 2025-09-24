namespace TrainingDay.Common.Communication;

/// <summary>
/// Model to send Firebase token to server with user settings
/// </summary>
public class FirebaseTokenDto
{
    public required string Token { get; set; }
    public required string Language { get; set; }
    public required string Zone { get; set; }
}
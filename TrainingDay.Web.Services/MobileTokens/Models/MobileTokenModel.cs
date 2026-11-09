namespace TrainingDay.Web.Services.MobileTokens.Models;

/// <summary>A device registered for push notifications.</summary>
public sealed record MobileTokenModel
{
    public int Id { get; init; }

    public required string Token { get; init; }

    public required string Language { get; init; }

    /// <summary>UTC offset of the device, e.g. "+03:00".</summary>
    public required string Zone { get; init; }

    /// <summary>UTC, last time the app was opened.</summary>
    public DateTime LastSend { get; init; }

    /// <summary>UTC, last workout.</summary>
    public DateTime LastWorkoutDateTime { get; init; }

    /// <summary>UTC, last body control (weight) entry.</summary>
    public DateTime LastBodyControlDateTime { get; init; }
}

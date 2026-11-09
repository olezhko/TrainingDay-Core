using TrainingDay.Web.Services.MobileTokens.Models;

namespace TrainingDay.Web.Services.UserTokens.Models;

/// <summary>A user and the devices linked to their account.</summary>
public sealed record UserTokenModel
{
    public IReadOnlyList<MobileTokenModel> Tokens { get; init; } = [];

    public string? UserMail { get; init; }

    public string? UserName { get; init; }

    public Guid UserId { get; init; }
}

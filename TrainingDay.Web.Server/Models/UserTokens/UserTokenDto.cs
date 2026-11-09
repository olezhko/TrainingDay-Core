using TrainingDay.Web.Server.Models.MobileTokens;

namespace TrainingDay.Web.Server.Models.UserTokens;

/// <summary>A user and the devices linked to their account.</summary>
public class UserTokenDto
{
    public IReadOnlyCollection<MobileTokenDto> Tokens { get; set; } = [];

    public string? UserMail { get; set; }

    public string? UserName { get; set; }

    public Guid UserId { get; set; }
}

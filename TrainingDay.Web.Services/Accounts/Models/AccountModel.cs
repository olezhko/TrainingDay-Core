using TrainingDay.Web.Services.Auth;

namespace TrainingDay.Web.Services.Accounts.Models;

/// <summary>A signed-up user.</summary>
/// <param name="Role">First Identity role of the user, "User" when none is assigned.</param>
public sealed record AccountModel(Guid Id, string? Email, string? Nickname, string Role, bool ShareCompletedWorkouts);

/// <summary>A successful password sign-in: the account plus the JWT pair for the mobile app.</summary>
public sealed record LoginModel(AccountModel Account, TokenResult Tokens);

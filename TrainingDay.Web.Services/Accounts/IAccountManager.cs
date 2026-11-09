using System.Security.Claims;
using TrainingDay.Web.Data.Results;
using TrainingDay.Web.Services.Accounts.Models;

namespace TrainingDay.Web.Services.Accounts;

/// <summary>
/// Account operations on top of ASP.NET Identity. Methods that send a confirmation email take
/// <c>confirmationUrl</c>: the absolute URL of the confirm endpoint, without query string.
/// </summary>
public interface IAccountManager
{
    /// <summary>The id of the signed-in user, or null when not signed in or the account no longer exists.</summary>
    Task<Guid?> GetUserIdAsync(ClaimsPrincipal principal);

    /// <summary>Fails with <see cref="AccountErrors.NotSignedIn"/>.</summary>
    Task<Result<AccountModel>> GetCurrentAsync(ClaimsPrincipal principal);

    /// <summary>Creates the account with the "User" role and sends the confirmation email.
    /// Fails with <see cref="AccountErrors.EmailTaken"/> or <see cref="AccountErrors.IdentityFailed"/>.</summary>
    Task<Result> RegisterAsync(string email, string password, string? nickname, string confirmationUrl);

    /// <summary>Signs in with the Identity cookie and issues JWTs. When the email is not confirmed, sends the
    /// confirmation email again and fails with <see cref="AccountErrors.EmailNotConfirmed"/>. Also fails with
    /// <see cref="AccountErrors.EmailNotFound"/>, <see cref="AccountErrors.LockedOut"/> or <see cref="AccountErrors.InvalidCredentials"/>.</summary>
    Task<Result<LoginModel>> LoginAsync(string email, string password, string confirmationUrl, CancellationToken token);

    Task SignOutAsync();

    /// <summary>Replaces the password with a random one and emails it (with a confirmation link if the email is not
    /// confirmed). Fails with <see cref="AccountErrors.EmailNotFound"/> or <see cref="AccountErrors.IdentityFailed"/>.</summary>
    Task<Result> ResetPasswordAsync(string email, string confirmationUrl);

    /// <summary>Fails with <see cref="AccountErrors.UserNotFound"/> or <see cref="AccountErrors.ConfirmationFailed"/>.</summary>
    Task<Result> ConfirmEmailAsync(string userId, string code);

    /// <summary>Changes the nickname (when given and different) and the sharing flag.
    /// Fails with <see cref="AccountErrors.NotSignedIn"/> or <see cref="AccountErrors.IdentityFailed"/>.</summary>
    Task<Result<AccountModel>> UpdateSettingsAsync(ClaimsPrincipal principal, string? nickname, bool shareCompletedWorkouts);

    /// <summary>Signs out and deletes the account. Fails with <see cref="AccountErrors.NotSignedIn"/> or <see cref="AccountErrors.IdentityFailed"/>.</summary>
    Task<Result> DeleteAsync(ClaimsPrincipal principal);
}

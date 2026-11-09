using Microsoft.AspNetCore.Identity;
using TrainingDay.Web.Data.Results;

namespace TrainingDay.Web.Services.Accounts;

public static class AccountErrors
{
    public static readonly Error NotSignedIn = Error.Unauthorized("Account.NotSignedIn", "The user is not signed in.");

    public static readonly Error UserNotFound = Error.NotFound("Account.UserNotFound", "User not found.");

    public static readonly Error EmailTaken = Error.Validation("Account.EmailTaken", "An account with this email already exists.");

    public static readonly Error EmailNotFound = Error.Validation("Account.EmailNotFound", "No account found with this email.");

    public static readonly Error EmailNotConfirmed = Error.Validation("Account.EmailNotConfirmed", "Please confirm your email before signing in.");

    public static readonly Error LockedOut = Error.Validation("Account.LockedOut", "Account is locked. Please try again later.");

    public static readonly Error InvalidCredentials = Error.Validation("Account.InvalidCredentials", "Invalid email or password.");

    public static readonly Error ConfirmationFailed = Error.Validation("Account.ConfirmationFailed", "Email confirmation failed.");

    /// <summary>An Identity operation failed; the description lists Identity's own messages.</summary>
    public static Error IdentityFailed(IdentityResult result) =>
        Error.Validation("Account.IdentityFailed", string.Join(" ", result.Errors.Select(e => e.Description)));
}

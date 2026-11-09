using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using TrainingDay.Web.Data.Results;
using TrainingDay.Web.Entities;
using TrainingDay.Web.Services.Accounts.Models;
using TrainingDay.Web.Services.Auth;
using TrainingDay.Web.Services.Email;

namespace TrainingDay.Web.Services.Accounts;

public class AccountManager(
    UserManager<MobileUser> userManager,
    SignInManager<MobileUser> signInManager,
    IJwtTokenService jwtTokenService,
    IEmailSender emailSender,
    ILogger<AccountManager> logger) : IAccountManager
{
    private const string DefaultRole = "User";

    public async Task<Guid?> GetUserIdAsync(ClaimsPrincipal principal)
    {
        var user = await userManager.GetUserAsync(principal);
        return user?.Id;
    }

    public async Task<Result<AccountModel>> GetCurrentAsync(ClaimsPrincipal principal)
    {
        var user = await userManager.GetUserAsync(principal);
        if (user == null)
        {
            return AccountErrors.NotSignedIn;
        }

        return await ToModelAsync(user);
    }

    public async Task<Result> RegisterAsync(string email, string password, string? nickname, string confirmationUrl)
    {
        if (await userManager.FindByEmailAsync(email) != null)
        {
            return AccountErrors.EmailTaken;
        }

        var user = new MobileUser { UserName = nickname, Email = email };
        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            return AccountErrors.IdentityFailed(result);
        }

        await userManager.AddToRoleAsync(user, DefaultRole);
        logger.LogInformation("User created a new account with password.");
        await emailSender.SendEmailConfirmationAsync(email, await CreateConfirmationLinkAsync(user, confirmationUrl));
        return Result.Success();
    }

    public async Task<Result<LoginModel>> LoginAsync(string email, string password, string confirmationUrl, CancellationToken token)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return AccountErrors.EmailNotFound;
        }

        if (!await userManager.IsEmailConfirmedAsync(user))
        {
            logger.LogWarning("User not confirmed.");
            await emailSender.SendEmailConfirmationAsync(email, await CreateConfirmationLinkAsync(user, confirmationUrl));
            return AccountErrors.EmailNotConfirmed;
        }

        var result = await signInManager.PasswordSignInAsync(user.UserName!, password, isPersistent: true, lockoutOnFailure: false);
        if (result.IsLockedOut)
        {
            logger.LogWarning("User account locked out.");
            return AccountErrors.LockedOut;
        }

        if (!result.Succeeded)
        {
            return AccountErrors.InvalidCredentials;
        }

        logger.LogInformation("User logged in.");
        var tokens = await jwtTokenService.IssueTokensAsync(user, token);
        return new LoginModel(await ToModelAsync(user), tokens);
    }

    public async Task SignOutAsync()
    {
        await signInManager.SignOutAsync();
        logger.LogInformation("User logged out.");
    }

    public async Task<Result> ResetPasswordAsync(string email, string confirmationUrl)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return AccountErrors.EmailNotFound;
        }

        var newPassword = GenerateRandomPassword();
        var resetCode = await userManager.GeneratePasswordResetTokenAsync(user);
        var resetResult = await userManager.ResetPasswordAsync(user, resetCode, newPassword);
        if (!resetResult.Succeeded)
        {
            return AccountErrors.IdentityFailed(resetResult);
        }

        if (!await userManager.IsEmailConfirmedAsync(user))
        {
            var confirmationLink = await CreateConfirmationLinkAsync(user, confirmationUrl);
            await emailSender.SendNewPasswordWithConfirmationAsync(email, newPassword, confirmationLink);
        }
        else
        {
            await emailSender.SendNewPasswordAsync(email, newPassword);
        }

        logger.LogInformation("Password reset for user {Email}", user.Email);
        return Result.Success();
    }

    public async Task<Result> ConfirmEmailAsync(string userId, string code)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return AccountErrors.UserNotFound;
        }

        var result = await userManager.ConfirmEmailAsync(user, code);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            logger.LogWarning("Email confirmation failed for user {Email}: {Errors}", user.Email, errors);
            return AccountErrors.ConfirmationFailed;
        }

        logger.LogInformation("Email confirmed for user {Email}", user.Email);
        return Result.Success();
    }

    public async Task<Result<AccountModel>> UpdateSettingsAsync(ClaimsPrincipal principal, string? nickname, bool shareCompletedWorkouts)
    {
        var user = await userManager.GetUserAsync(principal);
        if (user == null)
        {
            return AccountErrors.NotSignedIn;
        }

        if (!string.IsNullOrWhiteSpace(nickname) && nickname != user.UserName)
        {
            var result = await userManager.SetUserNameAsync(user, nickname);
            if (!result.Succeeded)
            {
                return AccountErrors.IdentityFailed(result);
            }

            // The cookie carries the user name, so reissue it.
            await signInManager.RefreshSignInAsync(user);
        }

        user.ShareCompletedWorkouts = shareCompletedWorkouts;
        await userManager.UpdateAsync(user);

        return await ToModelAsync(user);
    }

    public async Task<Result> DeleteAsync(ClaimsPrincipal principal)
    {
        var user = await userManager.GetUserAsync(principal);
        if (user == null)
        {
            return AccountErrors.NotSignedIn;
        }

        await signInManager.SignOutAsync();

        var result = await userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            return AccountErrors.IdentityFailed(result);
        }

        logger.LogInformation("Account removed for user {Email}", user.Email);
        return Result.Success();
    }

    private async Task<AccountModel> ToModelAsync(MobileUser user)
    {
        var roles = await userManager.GetRolesAsync(user);
        return new AccountModel(user.Id, user.Email, user.UserName, roles.FirstOrDefault() ?? DefaultRole, user.ShareCompletedWorkouts);
    }

    private async Task<string> CreateConfirmationLinkAsync(MobileUser user, string confirmationUrl)
    {
        var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
        return $"{confirmationUrl}?userId={WebUtility.UrlEncode(user.Id.ToString())}&code={WebUtility.UrlEncode(code)}";
    }

    private static string GenerateRandomPassword()
    {
        const string upper = "ABCDEFGHJKLMNPQRSTUVWXYZ";
        const string lower = "abcdefghijkmnopqrstuvwxyz";
        const string digits = "23456789";
        const string special = "!@#$%^&*";
        const string all = upper + lower + digits + special;

        var chars = new[]
        {
            upper[RandomNumberGenerator.GetInt32(upper.Length)],
            lower[RandomNumberGenerator.GetInt32(lower.Length)],
            digits[RandomNumberGenerator.GetInt32(digits.Length)],
            special[RandomNumberGenerator.GetInt32(special.Length)]
        }.ToList();

        for (int i = chars.Count; i < 12; i++)
            chars.Add(all[RandomNumberGenerator.GetInt32(all.Length)]);

        return new string(chars.OrderBy(_ => RandomNumberGenerator.GetInt32(int.MaxValue)).ToArray());
    }
}

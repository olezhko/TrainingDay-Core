using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Cryptography;
using TrainingDay.Web.Entities;
using TrainingDay.Web.Server.Extensions;
using TrainingDay.Web.Server.ViewModels.Auth;
using TrainingDay.Web.Services.Auth;
using TrainingDay.Web.Services.Email;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace TrainingDay.Web.Server.Controllers;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class AuthController(
    UserManager<MobileUser> userManager,
    SignInManager<MobileUser> signInManager,
    IJwtTokenService jwtTokenService,
    IEmailSender emailSender,
    ILogger<AuthController> logger) : ControllerBase
{
    const string callbackScheme = "trainingday";

    [HttpGet("{scheme}")]
    public async Task GetAsync([FromRoute] string scheme)
    {
        var auth = await Request.HttpContext.AuthenticateAsync(scheme);

        if (!auth.Succeeded
            || auth?.Principal == null
            || !auth.Principal.Identities.Any(id => id.IsAuthenticated)
            || string.IsNullOrEmpty(auth.Properties.GetTokenValue("access_token")))
        {
            // Not authenticated, challenge
            await Request.HttpContext.ChallengeAsync(scheme);
        }
        else
        {
            var claims = auth.Principal.Identities.FirstOrDefault()?.Claims;
            var email = string.Empty;
            email = claims?.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value;

            // Get parameters to send back to the callback
            var qs = new Dictionary<string, string>
            {
                { "access_token", auth.Properties.GetTokenValue("access_token") },
                { "refresh_token", auth.Properties.GetTokenValue("refresh_token") ?? string.Empty },
                { "expires", (auth.Properties.ExpiresUtc?.ToUnixTimeSeconds() ?? -1).ToString() },
                { "email", email }
            };

            // Build the result url
            var url = callbackScheme + "://#" + string.Join(
                          "&",
                          qs.Where(kvp => !string.IsNullOrEmpty(kvp.Value) && kvp.Value != "-1")
                              .Select(kvp => $"{WebUtility.UrlEncode(kvp.Key)}={WebUtility.UrlEncode(kvp.Value)}"));

            // Redirect to final url
            Request.HttpContext.Response.Redirect(url);
        }
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync(AuthRegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid registration data.");

        var existing = await userManager.FindByEmailAsync(model.Email);
        if (existing != null)
            return BadRequest("An account with this email already exists.");

        var user = new MobileUser { UserName = model.Nick, Email = model.Email };
        var result = await userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, "User");
            logger.LogInformation("User created a new account with password.");
            var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.EmailConfirmationLink(user.Id.ToString(), code, Request.Scheme);
            await emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl);
            return Ok();
        }

        var errors = string.Join(" ", result.Errors.Select(e => e.Description));
        return BadRequest(errors);
    }

    [HttpPost("forgot")]
    public async Task<IActionResult> ForgotPasswordAsync(ForgotPasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid email address.");

        var user = await userManager.FindByEmailAsync(model.Email);
        if (user == null)
            return BadRequest("No account found with this email.");

        var newPassword = GenerateRandomPassword();
        var resetCode = await userManager.GeneratePasswordResetTokenAsync(user);
        var resetResult = await userManager.ResetPasswordAsync(user, resetCode, newPassword);
        if (!resetResult.Succeeded)
        {
            var errors = string.Join(" ", resetResult.Errors.Select(e => e.Description));
            return BadRequest(errors);
        }

        if (!await userManager.IsEmailConfirmedAsync(user))
        {
            var confirmationCode = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationUrl = Url.EmailConfirmationLink(user.Id.ToString(), confirmationCode, Request.Scheme);
            await emailSender.SendNewPasswordWithConfirmationAsync(model.Email, newPassword, confirmationUrl);
        }
        else
        {
            await emailSender.SendNewPasswordAsync(model.Email, newPassword);
        }

        logger.LogInformation($"Password reset for user {user.Email}");
        return Ok();
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

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid login data.");

        var user = await userManager.FindByEmailAsync(model.Email);
        if (user == null)
            return BadRequest("No account found with this email.");

        if (!await userManager.IsEmailConfirmedAsync(user))
        {
            logger.LogWarning("User not confirmed.");
            var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.EmailConfirmationLink(user.Id.ToString(), code, Request.Scheme);
            await emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl);
            return BadRequest("Please confirm your email before signing in.");
        }

        SignInResult result = await signInManager.PasswordSignInAsync(user.UserName!, model.Password, true, lockoutOnFailure: false);
        if (result.Succeeded)
        {
            logger.LogInformation("User logged in.");
            var tokens = await jwtTokenService.IssueTokensAsync(user);
            var roles = await userManager.GetRolesAsync(user);
            return Ok(new
            {
                email = user.Email,
                nick = user.UserName,
                accessToken = tokens.AccessToken,
                accessTokenExpires = tokens.AccessTokenExpiresAt,
                refreshToken = tokens.RefreshToken,
                refreshTokenExpires = tokens.RefreshTokenExpiresAt,
                role = roles.FirstOrDefault() ?? "User"
            });
        }

        if (result.IsLockedOut)
        {
            logger.LogWarning("User account locked out.");
            return BadRequest("Account is locked. Please try again later.");
        }

        return BadRequest("Invalid email or password.");
    }

    [HttpPost("logout")]
    public async Task<IActionResult> LogoutAsync()
    {
        await signInManager.SignOutAsync();
        logger.LogInformation("User logged out.");
        return Ok();
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshAsync([FromBody] RefreshTokenViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var tokens = await jwtTokenService.RefreshAsync(model.RefreshToken, cancellationToken);
        if (tokens == null)
        {
            return Unauthorized("Invalid or expired refresh token.");
        }

        return Ok(new
        {
            accessToken = tokens.AccessToken,
            accessTokenExpires = tokens.AccessTokenExpiresAt,
            refreshToken = tokens.RefreshToken,
            refreshTokenExpires = tokens.RefreshTokenExpiresAt
        });
    }

    [HttpPost("revoke")]
    public async Task<IActionResult> RevokeAsync([FromBody] RefreshTokenViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await jwtTokenService.RevokeAsync(model.RefreshToken, cancellationToken);
        logger.LogInformation("Refresh token revoked.");
        return Ok();
    }

    [HttpDelete("remove")]
    [Authorize]
    public async Task<IActionResult> RemoveAccountAsync()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized();

        await signInManager.SignOutAsync();

        var result = await userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            var errors = string.Join(" ", result.Errors.Select(e => e.Description));
            return BadRequest(errors);
        }

        logger.LogInformation($"Account removed for user {user.Email}");
        return Ok();
    }

    [HttpGet("enter")]
    [Authorize]
    public async Task<IActionResult> EnterAsync()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null) 
            return Unauthorized();

        var roles = await userManager.GetRolesAsync(user);
        logger.LogInformation($"Enter {user.Email}");
        return Ok(new { email = user.Email, nick = user.UserName, role = roles.FirstOrDefault() ?? "User" });
    }

    [HttpGet("confirm")]
    public async Task<IActionResult> ConfirmEmailAsync(string userId, string code)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(code))
        {
            return BadRequest("Invalid confirmation link.");
        }

        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        var result = await userManager.ConfirmEmailAsync(user, code);
        if (result.Succeeded)
        {
            logger.LogInformation($"Email confirmed for user {user.Email}");
            return Ok();
        }

        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
        logger.LogWarning($"Email confirmation failed for user {user.Email}: {errors}");
        return BadRequest("Email confirmation failed.");
    }
}

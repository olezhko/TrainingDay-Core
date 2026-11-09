using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TrainingDay.Web.Server.Extensions;
using TrainingDay.Web.Server.ViewModels.Auth;
using TrainingDay.Web.Services.Accounts;
using TrainingDay.Web.Services.Auth;

namespace TrainingDay.Web.Server.Controllers;

// Account errors are returned as plain text (this.PlainError) because existing app versions read the message as a string.
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class AuthController(
    IAccountManager accountManager,
    IJwtTokenService jwtTokenService,
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

        var result = await accountManager.RegisterAsync(model.Email, model.Password, model.Nick, ConfirmationUrl);
        return result.IsSuccess ? Ok() : this.PlainError(result.Error);
    }

    [HttpPost("forgot")]
    public async Task<IActionResult> ForgotPasswordAsync(ForgotPasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid email address.");

        var result = await accountManager.ResetPasswordAsync(model.Email, ConfirmationUrl);
        return result.IsSuccess ? Ok() : this.PlainError(result.Error);
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(LoginViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid login data.");

        var result = await accountManager.LoginAsync(model.Email, model.Password, ConfirmationUrl, cancellationToken);
        if (result.IsFailure)
            return this.PlainError(result.Error);

        var (account, tokens) = result.Value;
        return Ok(new
        {
            email = account.Email,
            nick = account.Nickname,
            accessToken = tokens.AccessToken,
            accessTokenExpires = tokens.AccessTokenExpiresAt,
            refreshToken = tokens.RefreshToken,
            refreshTokenExpires = tokens.RefreshTokenExpiresAt,
            role = account.Role
        });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> LogoutAsync()
    {
        await accountManager.SignOutAsync();
        return Ok();
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshAsync([FromBody] RefreshTokenViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await jwtTokenService.RefreshAsync(model.RefreshToken, cancellationToken);
        if (result.IsFailure)
        {
            // Keep the original plain-text 401 body for existing app versions.
            return Unauthorized(result.Error.Description);
        }

        var token = result.Value;

        return Ok(new
        {
            accessToken = token.AccessToken,
            accessTokenExpires = token.AccessTokenExpiresAt,
            refreshToken = token.RefreshToken,
            refreshTokenExpires = token.RefreshTokenExpiresAt
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
        var result = await accountManager.DeleteAsync(User);
        return result.IsSuccess ? Ok() : this.PlainError(result.Error);
    }

    [HttpGet("enter")]
    [Authorize]
    public async Task<IActionResult> EnterAsync()
    {
        var result = await accountManager.GetCurrentAsync(User);
        if (result.IsFailure)
            return this.PlainError(result.Error);

        var account = result.Value;
        logger.LogInformation("Enter {Email}", account.Email);
        return Ok(new { email = account.Email, nick = account.Nickname, role = account.Role });
    }

    [HttpGet("confirm")]
    public async Task<IActionResult> ConfirmEmailAsync(string userId, string code)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(code))
        {
            return BadRequest("Invalid confirmation link.");
        }

        var result = await accountManager.ConfirmEmailAsync(userId, code);
        return result.IsSuccess ? Ok() : this.PlainError(result.Error);
    }

    private string ConfirmationUrl => Url.EmailConfirmationUrl(Request.Scheme);
}

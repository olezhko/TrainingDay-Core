 using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TrainingDay.Web.Entities;
using TrainingDay.Web.Server.Extensions;
using TrainingDay.Web.Server.ViewModels.Auth;
using TrainingDay.Web.Services.Email;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace TrainingDay.Web.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(
        UserManager<MobileUser> userManager,
        SignInManager<MobileUser> signInManager,
        IEmailSender emailSender,
        ILogger<AuthController> logger) : ControllerBase
    {
        const string callbackScheme = "trainingday";
        private readonly ILogger _logger = logger;

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
                _logger.LogInformation("User created a new account with password.");
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

            if (!await userManager.IsEmailConfirmedAsync(user))
                return BadRequest("This account has not been confirmed yet. Please check your email for the confirmation link.");

            var code = await userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.ResetPasswordCallbackLink(user.Id.ToString(), code, Request.Scheme);
            await emailSender.SendEmailAsync(model.Email, "Reset Password", $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");
            return Ok();
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
                _logger.LogWarning("User not confirmed.");
                return BadRequest("Please confirm your email before signing in.");
            }

            SignInResult result = await signInManager.PasswordSignInAsync(user.UserName!, model.Password, true, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                _logger.LogInformation("User logged in.");
                return Ok();
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning("User account locked out.");
                return BadRequest("Account is locked. Please try again later.");
            }

            return BadRequest("Invalid email or password.");
        }

        [HttpPost("logout")]
        public async Task<IActionResult> LogoutAsync()
        {
            await signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return Ok();
        }

        [HttpGet("enter")]
        [Authorize]
        public IActionResult Enter()
        {
            _logger.LogInformation($"Enter {User.Identity!.Name}");
            return Ok();
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
                _logger.LogInformation($"Email confirmed for user {user.Email}");
                return Ok();
            }

            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            _logger.LogWarning($"Email confirmation failed for user {user.Email}: {errors}");
            return BadRequest("Email confirmation failed.");
        }

        [HttpPost("reset")]
        public async Task<IActionResult> ResetPasswordAsync([FromBody] ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var result = await userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                _logger.LogInformation($"Password reset for user {user.Email}");
                return Ok();
            }

            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return BadRequest(errors);
        }
    }
}

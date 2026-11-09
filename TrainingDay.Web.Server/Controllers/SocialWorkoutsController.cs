using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TrainingDay.Common.Models;
using TrainingDay.Web.Server.Extensions;
using TrainingDay.Web.Services.Accounts;
using TrainingDay.Web.Services.SocialWorkouts;

namespace TrainingDay.Web.Server.Controllers;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[Authorize]
public class SocialWorkoutsController(ISocialWorkoutManager manager, IAccountManager accountManager) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetFeedAsync([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken token = default)
    {
        var userId = await accountManager.GetUserIdAsync(User);

        var result = await manager.GetFeedAsync(userId, page, pageSize, token);
        return Ok(result);
    }

    [HttpPost]
    [AllowAnonymous]
    [EnableRateLimiting(RateLimitingExtensions.ShareWorkoutPolicy)]
    public async Task<IActionResult> ShareAsync([FromBody] ShareSocialWorkoutRequest request, CancellationToken token)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userId = await accountManager.GetUserIdAsync(User);

        var result = await manager.ShareAsync(userId, request, token);
        if (result.IsFailure)
        {
            // Keep the original plain-text 400 body for existing app versions.
            return result.Error == SocialWorkoutErrors.SharingDisabled
                ? BadRequest(result.Error.Description)
                : this.Problem(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpPost("{id:int}/like")]
    public async Task<IActionResult> LikeAsync([FromRoute] int id, CancellationToken token)
    {
        var userId = await accountManager.GetUserIdAsync(User);
        if (userId == null)
        {
            return Unauthorized();
        }

        var result = await manager.LikeAsync(userId.Value, id, token);
        return result.IsSuccess ? Ok() : this.Problem(result.Error);
    }

    [HttpDelete("{id:int}/like")]
    public async Task<IActionResult> UnlikeAsync([FromRoute] int id, CancellationToken token)
    {
        var userId = await accountManager.GetUserIdAsync(User);
        if (userId == null)
        {
            return Unauthorized();
        }

        var result = await manager.UnlikeAsync(userId.Value, id, token);
        return result.IsSuccess ? Ok() : this.Problem(result.Error);
    }
}

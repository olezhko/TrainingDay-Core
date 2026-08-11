using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TrainingDay.Common.Models;
using TrainingDay.Web.Entities;
using TrainingDay.Web.Services.SocialWorkouts;

namespace TrainingDay.Web.Server.Controllers;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[Authorize]
public class SocialWorkoutsController(ISocialWorkoutManager manager, UserManager<MobileUser> userManager) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetFeedAsync([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken token = default)
    {
        var user = await userManager.GetUserAsync(User);

        var result = await manager.GetFeedAsync(user?.Id, page, pageSize, token);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> ShareAsync([FromBody] ShareSocialWorkoutRequest request, CancellationToken token)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        try
        {
            var result = await manager.ShareAsync(user.Id, request, token);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id:int}/like")]
    public async Task<IActionResult> LikeAsync([FromRoute] int id, CancellationToken token)
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var success = await manager.LikeAsync(user.Id, id, token);
        return success ? Ok() : NotFound();
    }

    [HttpDelete("{id:int}/like")]
    public async Task<IActionResult> UnlikeAsync([FromRoute] int id, CancellationToken token)
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var success = await manager.UnlikeAsync(user.Id, id, token);
        return success ? Ok() : NotFound();
    }
}

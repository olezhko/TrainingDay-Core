using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TrainingDay.Common.Models;
using TrainingDay.Web.Entities;
using TrainingDay.Web.Services.UserRepo;

namespace TrainingDay.Web.Server.Controllers;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[Authorize]
public class UserRepoController(IUserRepoManager manager, UserManager<MobileUser> userManager) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAsync(CancellationToken token)
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var data = await manager.GetAsync(user.Id, token);
        return Ok(data);
    }

    [HttpPost]
    public async Task<IActionResult> UpsertAsync([FromBody] RepositoryBase model, CancellationToken token)
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

        await manager.UpsertAsync(user.Id, model, token);
        return Ok();
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrainingDay.Common.Models;
using TrainingDay.Web.Services.Accounts;
using TrainingDay.Web.Services.UserRepo;

namespace TrainingDay.Web.Server.Controllers;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[Authorize]
public class UserRepoController(IUserRepoManager manager, IAccountManager accountManager) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAsync(CancellationToken token)
    {
        var userId = await accountManager.GetUserIdAsync(User);
        if (userId == null)
        {
            return Unauthorized();
        }

        var data = await manager.GetAsync(userId.Value, token);
        return Ok(data);
    }

    [HttpPost]
    public async Task<IActionResult> UpsertAsync([FromBody] RepositoryBase model, CancellationToken token)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userId = await accountManager.GetUserIdAsync(User);
        if (userId == null)
        {
            return Unauthorized();
        }

        await manager.UpsertAsync(userId.Value, model, token);
        return Ok();
    }
}

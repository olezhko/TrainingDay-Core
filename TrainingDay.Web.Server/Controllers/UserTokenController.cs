using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrainingDay.Common.Communication;
using TrainingDay.Web.Server.Extensions;
using TrainingDay.Web.Server.Mapper;
using TrainingDay.Web.Server.Models.UserTokens;
using TrainingDay.Web.Services.UserTokens;

namespace TrainingDay.Web.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserTokenController(IUserTokenManager manager) : ControllerBase
    {
        // Lists every user's email, so admins only.
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<List<UserTokenDto>> GetItems(int page, int pageSize, CancellationToken cancellationToken)
        {
            var items = await manager.GetItemsAsync(page, pageSize, cancellationToken);
            return [.. items.Select(item => item.ToDto())];
        }

        [HttpPost("connect")]
        public async Task<IActionResult> ConnectTokenUser([FromBody] MobileUserToken repo, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await manager.ConnectTokenUser(repo, cancellationToken);
            if (result.IsFailure)
            {
                return this.Problem(result.Error);
            }

            return Ok();
        }
    }
}

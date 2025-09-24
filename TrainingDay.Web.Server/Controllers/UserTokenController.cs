using Microsoft.AspNetCore.Mvc;
using TrainingDay.Common.Communication;
using TrainingDay.Web.Data.UserToken;
using TrainingDay.Web.Services.UserTokens;

namespace TrainingDay.Web.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserTokenController(IUserTokenManager manager) : ControllerBase
    {
        [HttpGet]
        public async Task<List<UserTokenModel>> GetItems(int page, int pageSize)
        {
            return await manager.GetItems(page, pageSize);
        }

        [HttpPost("connect")]
        public async Task<IActionResult> ConnectTokenUser([FromBody] MobileUserToken repo, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            await manager.ConnectTokenUser(repo, cancellationToken);

            return Ok();
        }
    }
}

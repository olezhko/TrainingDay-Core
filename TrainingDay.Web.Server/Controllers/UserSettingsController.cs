using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrainingDay.Common.Models;
using TrainingDay.Web.Server.Extensions;
using TrainingDay.Web.Services.Accounts;
using TrainingDay.Web.Services.Accounts.Models;

namespace TrainingDay.Web.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserSettingsController(IAccountManager accountManager) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var result = await accountManager.GetCurrentAsync(User);
            return result.IsSuccess ? Ok(ToDto(result.Value)) : this.PlainError(result.Error);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateUserSettingsRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Plain-text errors, as before, for existing app versions.
            var result = await accountManager.UpdateSettingsAsync(User, model.Nickname, model.ShareCompletedWorkouts);
            return result.IsSuccess ? Ok(ToDto(result.Value)) : this.PlainError(result.Error);
        }

        private static UserSettingsDto ToDto(AccountModel account) => new UserSettingsDto
        {
            Nickname = account.Nickname,
            ShareCompletedWorkouts = account.ShareCompletedWorkouts,
        };
    }
}

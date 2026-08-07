using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TrainingDay.Common.Models;
using TrainingDay.Web.Entities;

namespace TrainingDay.Web.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserSettingsController(UserManager<MobileUser> userManager, SignInManager<MobileUser> signInManager) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            return Ok(new UserSettingsDto
            {
                Nickname = user.UserName,
                ShareCompletedWorkouts = user.ShareCompletedWorkouts,
            });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateUserSettingsRequest model)
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

            if (!string.IsNullOrWhiteSpace(model.Nickname) && model.Nickname != user.UserName)
            {
                var result = await userManager.SetUserNameAsync(user, model.Nickname);
                if (!result.Succeeded)
                {
                    return BadRequest(string.Join(" ", result.Errors.Select(e => e.Description)));
                }

                await signInManager.RefreshSignInAsync(user);
            }

            user.ShareCompletedWorkouts = model.ShareCompletedWorkouts;
            await userManager.UpdateAsync(user);

            return Ok(new UserSettingsDto
            {
                Nickname = user.UserName,
                ShareCompletedWorkouts = user.ShareCompletedWorkouts,
            });
        }
    }
}

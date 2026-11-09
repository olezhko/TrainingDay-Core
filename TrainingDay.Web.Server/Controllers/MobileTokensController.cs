using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrainingDay.Common.Communication;
using TrainingDay.Web.Server.Extensions;
using TrainingDay.Web.Server.Mapper;
using TrainingDay.Web.Server.Models.MobileTokens;
using TrainingDay.Web.Services.MobileTokens;

namespace TrainingDay.Web.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MobileTokensController(IMobileTokenManager manager) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> PostMobileTokenAsync([FromBody] FirebaseTokenDto firebaseToken, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await manager.RegisterAsync(firebaseToken, cancellationToken);
            return Ok();
        }

        [HttpPost("action")]
        public async Task<IActionResult> PostMobileActionAsync([FromBody] MobileActionDto token, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await manager.RecordActionAsync(token, cancellationToken);
            if (result.IsFailure)
            {
                // Keep the original 404 body (the posted action) for existing app versions.
                return result.Error == MobileTokenErrors.NotFound ? NotFound(token) : this.Problem(result.Error);
            }

            return Ok();
        }

#if !DEBUG
        [Authorize(Roles = "Admin")]
#endif

        [HttpGet]
        public async Task<IActionResult> GetMobileTokensAsync([FromQuery] GetMobileTokensFilter filter, CancellationToken cancellationToken)
        {
            var tokens = await manager.GetAllAsync(filter.LastActiveDays, cancellationToken);
            var ret = tokens.Select(item => item.ToDto()).ToList();

            return Ok(new MobileTokensDto()
            {
                MobileTokens = ret,
                TotalCount = ret.Count
            });
        }

        [HttpGet("{token}")]
#if !DEBUG
        [Authorize(Roles = "Admin")]
#endif
        public async Task<IActionResult> GetMobileTokenAsync([FromRoute] string token, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await manager.GetAsync(token, cancellationToken);
            if (result.IsFailure)
            {
                return this.Problem(result.Error);
            }

            return Ok(result.Value.ToDto());
        }

        [HttpDelete("{token}")]
#if !DEBUG
        [Authorize(Roles = "Admin")]
#endif
        public async Task<IActionResult> DeleteAsync([FromRoute] string token, CancellationToken cancellationToken)
        {
            var result = await manager.DeleteAsync(token, cancellationToken);
            if (result.IsFailure)
            {
                return this.Problem(result.Error);
            }

            return Ok(result.Value.ToDto());
        }

        [HttpDelete("unused")]
#if !DEBUG
        [Authorize(Roles = "Admin")]
#endif
        public async Task<IActionResult> RemoveUnusedTokensAsync(CancellationToken cancellationToken)
        {
            await manager.RemoveUnusedAsync(cancellationToken);
            return Ok();
        }
    }
}

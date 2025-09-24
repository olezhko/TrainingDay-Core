using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingDay.Common.Communication;
using TrainingDay.Web.Database;
using TrainingDay.Web.Entities;
using TrainingDay.Web.Server.Models.MobileTokens;
using TrainingDay.Web.Services.Extensions;

namespace TrainingDay.Web.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MobileTokensController(TrainingDayContext context) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> PostMobileTokenAsync([FromBody] FirebaseTokenDto firebaseToken, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!DateTimeExtensions.TryParseZone(firebaseToken.Zone, out var zone))
            {
                ModelState.AddModelError("Zone", "Invalid zone format. Expected format is ±hh:mm (e.g., +03:00 or -05:30).");
                return BadRequest(ModelState);
            }

            var token = context.MobileTokens.FirstOrDefault(item => item.Token == firebaseToken.Token);
            if (token != null)
            {
                token.Language = firebaseToken.Language;
                token.Zone = firebaseToken.Zone.ToString();
                token.LastSend = DateTime.Now;
                context.Update(token);
                await context.SaveChangesAsync(cancellationToken);
                return Ok();
            }

            MobileToken newItem = new()
            {
                Token = firebaseToken.Token,
                Zone = firebaseToken.Zone.ToString(),
                Language = firebaseToken.Language,
                LastSend = DateTime.Now
            };

            context.MobileTokens.Add(newItem);
            await context.SaveChangesAsync(cancellationToken);
            return Ok();
        }

        [HttpPost("action")]
        public async Task<IActionResult> PostMobileActionAsync([FromBody] MobileActionDto token, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var mobileToken = await context.MobileTokens.SingleOrDefaultAsync(m => m.Token == token.Token);
            if (mobileToken == null)
            {
                return NotFound(token);
            }

            switch (token.Action)
            {
                case MobileActions.Enter:
                    mobileToken.LastSend = DateTime.UtcNow;
                    break;
                case MobileActions.Workout:
                    mobileToken.LastWorkoutDateTime = DateTime.UtcNow;
                    break;
                case MobileActions.Weight:
                    mobileToken.LastBodyControlDateTime = DateTime.UtcNow;
                    break;
                default:
                    break;
            }

            context.Update(mobileToken);
            await context.SaveChangesAsync(cancellationToken);

            return Ok();
        }

#if !DEBUG
        [Authorize(Roles = "admin")]
#endif

        [HttpGet]
        public async Task<IActionResult> GetMobileTokensAsync([FromQuery] GetMobileTokensFilter filter, CancellationToken cancellationToken)
        {
            var query = context.MobileTokens
                    .AsNoTracking();

            if (filter.LastActiveDays is not null)
            {
                query = query.Where(item => (DateTime.Now - item.LastSend) < TimeSpan.FromDays(filter.LastActiveDays.Value));
            }

            var ret = await query
                .Select(item => new MobileTokenDto()
                {
                    Language = item.Language,
                    Token = item.Token,
                    Zone = item.Zone,
                    LastSend = item.LastSend,
                    Id = item.Id,
                    LastBodyControlDateTime = item.LastBodyControlDateTime,
                    LastWorkoutDateTime = item.LastWorkoutDateTime
                })
                .ToListAsync(cancellationToken);

            return Ok(new MobileTokensDto()
            {
                MobileTokens = ret,
                TotalCount = ret.Count
            });
        }

        [HttpGet("{token}")]
#if !DEBUG
        [Authorize(Roles = "admin")]
#endif
        public async Task<IActionResult> GetMobileTokenAsync([FromRoute] string token, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var mobileToken = await context.MobileTokens.SingleOrDefaultAsync(m => m.Token == token, cancellationToken);

            if (mobileToken == null)
            {
                return NotFound();
            }

            return Ok(mobileToken);
        }

        [HttpDelete("{token}")]
#if !DEBUG
        [Authorize(Roles = "admin")]
#endif
        public async Task<IActionResult> DeleteAsync([FromRoute] string token, CancellationToken cancellationToken)
        {
            var mobileToken = await context.MobileTokens.SingleOrDefaultAsync(m => m.Token == token, cancellationToken);
            if (mobileToken == null)
            {
                return NotFound(token);
            }

            context.MobileTokens.Remove(mobileToken);
            await context.SaveChangesAsync(cancellationToken);

            return Ok(mobileToken);
        }

        [HttpDelete("unused")]
#if !DEBUG
        [Authorize(Roles = "admin")]
#endif
        public async Task<IActionResult> RemoveUnusedTokensAsync(CancellationToken cancellationToken)
        {
            foreach (var contextMobileToken in context.MobileTokens)
            {
                if (contextMobileToken.LastSend == new DateTime())
                {
                    context.MobileTokens.Remove(contextMobileToken);
                    continue;
                }
            }

            await context.SaveChangesAsync(cancellationToken);
            return Ok();
        }
    }
}

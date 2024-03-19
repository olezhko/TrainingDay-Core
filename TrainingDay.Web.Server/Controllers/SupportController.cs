using Microsoft.AspNetCore.Mvc;
using TrainingDay.Web.Data.Support;
using TrainingDay.Web.Services.Support;

namespace TrainingDay.Web.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SupportController(ISupportManager manager) : ControllerBase
    {
        [HttpPost("contact-me")]
        public async Task<IActionResult> Contact([FromBody] ContactMeModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(request);
            }

            await manager.SendContactMe(request);

            return Ok();
        }
    }
}

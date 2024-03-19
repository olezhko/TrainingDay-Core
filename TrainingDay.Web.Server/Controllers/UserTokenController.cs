using Microsoft.AspNetCore.Mvc;
using TrainingDay.Web.Data.UserToken;
using TrainingDay.Web.Services.UserTokens;

namespace TrainingDay.Web.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserTokenController : ControllerBase
    {
        private readonly IUserTokenManager _manager;

        public UserTokenController(IUserTokenManager manager)
        {
            _manager = manager;
        }

        [HttpGet]
        public async Task<List<UserTokenModel>> GetItems(int page, int pageSize)
        {
            return await _manager.GetItems(page, pageSize);
        }
    }
}

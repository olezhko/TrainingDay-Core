using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TrainingDay.Web.Data.UserToken;
using TrainingDay.Web.Database;
using TrainingDay.Web.Entities;
using TrainingDay.Web.Entities.MobileItems;

namespace TrainingDay.Web.Services.UserTokens;

public class UserTokenManager(TrainingDayContext context, ILogger<UserTokenManager> logger) : IUserTokenManager
{
    public async Task<List<UserTokenModel>> GetItems(int page, int pageSize)
    {
        List<UserTokenModel> result = new List<UserTokenModel>();
        List<UserMobileToken> userTokens = await context.UserTokens.AsNoTracking()
            .Skip((page-1)*pageSize).Take(pageSize)
            .ToListAsync();
        foreach (UserMobileToken userToken in userTokens)
        {
            var user = await context.Users.FirstOrDefaultAsync(a => a.Id == userToken.UserId);
            List<MobileToken> tokens = await context.MobileTokens.AsNoTracking().Where(item => item.Id == userToken.TokenId).ToListAsync();
            result.Add(new UserTokenModel()
            {
                Tokens = tokens,
                UserMail = user.Email,
                UserName = user.Name,
                UserId = user.Id,
            });
        }

        return result;
    }

    public async Task RemoveNotExistToken(MobileToken token)
    {
        //IQueryable<UserAlarm> alarms = context.UserAlarm.Include(item => item.User).Where(item => item.TokenId == token.Id);
        //context.UserAlarm.RemoveRange(alarms);
        logger.LogInformation($"Remove Alarms");
        UserMobileToken userToken = await context.UserTokens.FirstOrDefaultAsync(item => item.TokenId == token.Id);
        if (userToken != null)
        {
            logger.LogInformation($"Remove UserToken {userToken.Id}");
            context.UserTokens.Remove(userToken);

            var user = await context.Users.FindAsync(userToken.UserId);
            if (user != null)
            {
                logger.LogInformation($"Remove user {user.Id}");
                context.Remove(user);
            }
        }

        context.MobileTokens.Remove(token);
        logger.LogInformation($"Remove MobileToken {token.Token} {token.Id}");

        await context.SaveChangesAsync();
    }
}
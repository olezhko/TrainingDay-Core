using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TrainingDay.Common.Communication;
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
            .Skip((page - 1) * pageSize).Take(pageSize)
            .ToListAsync();

        foreach (UserMobileToken userToken in userTokens)
        {
            var user = await context.MobileUsers.FirstOrDefaultAsync(a => a.Id == userToken.UserId);
            List<MobileToken> tokens = await context.MobileTokens.AsNoTracking().Where(item => item.Id == userToken.TokenId).ToListAsync();
            result.Add(new UserTokenModel()
            {
                Tokens = tokens,
                UserMail = user.Email,
                UserName = user.UserName,
                UserId = user.Id,
            });
        }

        return result;
    }

    public async Task RemoveNotExistToken(MobileToken token)
    {
        UserMobileToken userToken = await context.UserTokens.FirstOrDefaultAsync(item => item.TokenId == token.Id);
        if (userToken != null)
        {
            logger.LogInformation($"Remove UserToken {userToken.Id}");
            context.UserTokens.Remove(userToken);

            var user = await context.MobileUsers.FindAsync(userToken.UserId);
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

    public async Task ConnectTokenUser(MobileUserToken repo, CancellationToken token)
    {
        var mobileToken = await context.MobileTokens.FirstOrDefaultAsync(a => a.Token == repo.Token);
        if (mobileToken is null)
        {
            throw new KeyNotFoundException($"Token not found: {repo.Token}");
        }

        var user = await context.MobileUsers.FirstOrDefaultAsync(a => a.Id == repo.UserId);
        if (user is null)
        {
            throw new KeyNotFoundException($"User not found: {repo.UserId}");
        }

        var userExist = context.UserTokens.FirstOrDefault(item => item.UserId == user.Id);
        if (userExist is null)
        {
            var newUserToken = new UserMobileToken()
            {
                TokenId = mobileToken.Id,
                UserId = user.Id,
            };

            context.UserTokens.Add(newUserToken);
        }
        else
        {
            userExist.TokenId = mobileToken.Id;
            context.Update(userExist);
        }

        await context.SaveChangesAsync(token);
    }
}
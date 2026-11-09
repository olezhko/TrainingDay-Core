using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TrainingDay.Common.Communication;
using TrainingDay.Web.Data.Results;
using TrainingDay.Web.Database;
using TrainingDay.Web.Entities;
using TrainingDay.Web.Entities.MobileItems;
using TrainingDay.Web.Services.MobileTokens.Models;
using TrainingDay.Web.Services.UserTokens.Models;

namespace TrainingDay.Web.Services.UserTokens;

public class UserTokenManager(TrainingDayContext context, ILogger<UserTokenManager> logger) : IUserTokenManager
{
    public async Task<List<UserTokenModel>> GetItemsAsync(int page, int pageSize, CancellationToken token)
    {
        // One query instead of two extra queries per row.
        return await context.UserTokens.AsNoTracking()
            .OrderBy(item => item.Id)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .Select(item => new UserTokenModel()
            {
                Tokens = context.MobileTokens
                    .Where(mobileToken => mobileToken.Id == item.TokenId)
                    .Select(mobileToken => new MobileTokenModel
                    {
                        Id = mobileToken.Id,
                        Token = mobileToken.Token,
                        Language = mobileToken.Language,
                        Zone = mobileToken.Zone,
                        LastSend = mobileToken.LastSend,
                        LastWorkoutDateTime = mobileToken.LastWorkoutDateTime,
                        LastBodyControlDateTime = mobileToken.LastBodyControlDateTime,
                    })
                    .ToList(),
                UserMail = item.User.Email,
                UserName = item.User.UserName,
                UserId = item.User.Id,
            })
            .ToListAsync(token);
    }

    public async Task RemoveNotExistToken(MobileToken token)
    {
        // Only the device link goes away; the user account and its synced data are kept.
        UserMobileToken userToken = await context.UserTokens.FirstOrDefaultAsync(item => item.TokenId == token.Id);
        if (userToken != null)
        {
            logger.LogInformation($"Remove UserToken {userToken.Id}");
            context.UserTokens.Remove(userToken);
        }

        context.MobileTokens.Remove(token);
        logger.LogInformation($"Remove MobileToken {token.Token} {token.Id}");

        await context.SaveChangesAsync();
    }

    public async Task<Result> ConnectTokenUser(MobileUserToken repo, CancellationToken token)
    {
        var mobileToken = await context.MobileTokens.FirstOrDefaultAsync(a => a.Token == repo.Token, token);
        if (mobileToken is null)
        {
            return UserTokenErrors.TokenNotFound;
        }

        var user = await context.MobileUsers.FirstOrDefaultAsync(a => a.Id == repo.UserId, token);
        if (user is null)
        {
            return UserTokenErrors.UserNotFound;
        }

        var userExist = await context.UserTokens.FirstOrDefaultAsync(item => item.UserId == user.Id, token);
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
        }

        await context.SaveChangesAsync(token);
        return Result.Success();
    }
}
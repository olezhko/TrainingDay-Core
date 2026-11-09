using TrainingDay.Common.Communication;
using TrainingDay.Web.Data.Results;
using TrainingDay.Web.Entities;
using TrainingDay.Web.Services.UserTokens.Models;

namespace TrainingDay.Web.Services.UserTokens;
public interface IUserTokenManager
{
    Task<List<UserTokenModel>> GetItemsAsync(int page, int pageSize, CancellationToken token);
    Task RemoveNotExistToken(MobileToken token);
    /// <summary>Links the device token to the user. Fails with <see cref="UserTokenErrors.TokenNotFound"/> or <see cref="UserTokenErrors.UserNotFound"/>.</summary>
    Task<Result> ConnectTokenUser(MobileUserToken repo, CancellationToken token);
}
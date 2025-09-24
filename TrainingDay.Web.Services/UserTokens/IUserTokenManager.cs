using TrainingDay.Common.Communication;
using TrainingDay.Web.Data.UserToken;
using TrainingDay.Web.Entities;

namespace TrainingDay.Web.Services.UserTokens;
public interface IUserTokenManager
{
    Task<List<UserTokenModel>> GetItems(int page, int pageSize);
    Task RemoveNotExistToken(MobileToken token);
    Task ConnectTokenUser(MobileUserToken repo, CancellationToken token);
}
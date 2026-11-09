using TrainingDay.Web.Data.Results;

namespace TrainingDay.Web.Services.UserTokens;

public static class UserTokenErrors
{
    public static readonly Error TokenNotFound = Error.NotFound("UserTokens.TokenNotFound", "Device token not found.");

    public static readonly Error UserNotFound = Error.NotFound("UserTokens.UserNotFound", "User not found.");
}

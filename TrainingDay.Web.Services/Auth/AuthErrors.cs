using TrainingDay.Web.Data.Results;

namespace TrainingDay.Web.Services.Auth;

public static class AuthErrors
{
    public static readonly Error InvalidRefreshToken = Error.Unauthorized("Auth.InvalidRefreshToken", "Invalid or expired refresh token.");
}

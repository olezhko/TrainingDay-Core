using TrainingDay.Web.Data.Results;
using TrainingDay.Web.Entities;

namespace TrainingDay.Web.Services.Auth;

public class TokenResult
{
    public string AccessToken { get; set; }
    public DateTime AccessTokenExpiresAt { get; set; }
    public string RefreshToken { get; set; }
    public DateTime RefreshTokenExpiresAt { get; set; }
}

public interface IJwtTokenService
{
    Task<TokenResult> IssueTokensAsync(MobileUser user, CancellationToken cancellationToken = default);
    /// <summary>Fails with <see cref="AuthErrors.InvalidRefreshToken"/> when the token is unknown, revoked or expired.</summary>
    Task<Result<TokenResult>> RefreshAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task RevokeAsync(string refreshToken, CancellationToken cancellationToken = default);
}

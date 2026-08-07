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
    Task<TokenResult?> RefreshAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task RevokeAsync(string refreshToken, CancellationToken cancellationToken = default);
}

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TrainingDay.Web.Database;
using TrainingDay.Web.Entities;
using TrainingDay.Web.Entities.MobileItems;

namespace TrainingDay.Web.Services.Auth;

public class JwtTokenService(
    TrainingDayContext context,
    UserManager<MobileUser> userManager,
    IOptions<JwtSettings> options) : IJwtTokenService
{
    private readonly JwtSettings settings = options.Value;

    public async Task<TokenResult> IssueTokensAsync(MobileUser user, CancellationToken cancellationToken = default)
    {
        var roles = await userManager.GetRolesAsync(user);
        var accessToken = GenerateAccessToken(user, roles, out var accessExpiresAt);
        var refreshToken = CreateRefreshTokenEntity(user.Id);

        context.RefreshTokens.Add(refreshToken);
        await context.SaveChangesAsync(cancellationToken);

        return new TokenResult
        {
            AccessToken = accessToken,
            AccessTokenExpiresAt = accessExpiresAt,
            RefreshToken = refreshToken.Token,
            RefreshTokenExpiresAt = refreshToken.ExpiresAt
        };
    }

    public async Task<TokenResult?> RefreshAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var existing = await context.RefreshTokens
            .Include(item => item.User)
            .FirstOrDefaultAsync(item => item.Token == refreshToken, cancellationToken);

        if (existing == null || !existing.IsActive)
        {
            return null;
        }

        var newRefreshToken = CreateRefreshTokenEntity(existing.UserId);
        existing.RevokedAt = DateTime.UtcNow;
        existing.ReplacedByToken = newRefreshToken.Token;
        context.RefreshTokens.Add(newRefreshToken);

        var roles = await userManager.GetRolesAsync(existing.User);
        var accessToken = GenerateAccessToken(existing.User, roles, out var accessExpiresAt);

        await context.SaveChangesAsync(cancellationToken);

        return new TokenResult
        {
            AccessToken = accessToken,
            AccessTokenExpiresAt = accessExpiresAt,
            RefreshToken = newRefreshToken.Token,
            RefreshTokenExpiresAt = newRefreshToken.ExpiresAt
        };
    }

    public async Task RevokeAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var existing = await context.RefreshTokens
            .FirstOrDefaultAsync(item => item.Token == refreshToken, cancellationToken);

        if (existing is { IsActive: true })
        {
            existing.RevokedAt = DateTime.UtcNow;
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    private string GenerateAccessToken(MobileUser user, IList<string> roles, out DateTime expiresAt)
    {
        expiresAt = DateTime.UtcNow.AddMinutes(settings.AccessTokenExpirationMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(ClaimTypes.Name, user.UserName ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: settings.Issuer,
            audience: settings.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private RefreshToken CreateRefreshTokenEntity(Guid userId)
    {
        return new RefreshToken
        {
            UserId = userId,
            Token = GenerateSecureToken(),
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(settings.RefreshTokenExpirationDays)
        };
    }

    private static string GenerateSecureToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }
}

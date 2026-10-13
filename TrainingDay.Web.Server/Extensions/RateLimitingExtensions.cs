using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using System.Threading.RateLimiting;

namespace TrainingDay.Web.Server.Extensions;

public static class RateLimitingExtensions
{
    public const string ShareWorkoutPolicy = "share-workout";

    /// <summary>
    /// Sharing a workout is allowed without login, so limit it per user (logged in) or per IP (anonymous).
    /// UseRateLimiter must run after UseAuthorization so HttpContext.User is already authenticated (cookie or JWT).
    /// </summary>
    public static IServiceCollection AddAppRateLimiting(this IServiceCollection services)
    {
        return services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.AddPolicy(ShareWorkoutPolicy, context =>
            {
                var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId != null)
                {
                    return RateLimitPartition.GetFixedWindowLimiter($"user:{userId}", _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 20,
                        Window = TimeSpan.FromMinutes(10),
                    });
                }

                var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                return RateLimitPartition.GetFixedWindowLimiter($"ip:{ip}", _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 5,
                    Window = TimeSpan.FromMinutes(10),
                });
            });
        });
    }
}

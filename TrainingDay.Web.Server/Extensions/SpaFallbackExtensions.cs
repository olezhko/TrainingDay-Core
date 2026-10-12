using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using TrainingDay.Web.Database;

namespace TrainingDay.Web.Server.Extensions;

/// <summary>
/// Prerendered pages are served by the static files middleware. For everything else this serves the
/// client-side shell (index.csr.html, see trainingday.web.client/scripts/create-csr-shell.mjs) with
/// HTTP 404 for unknown paths and missing blog posts / exercises, so search engines don't index them
/// as "soft 404" pages.
/// Keep the route list in sync with trainingday.web.client/src/app/app.routes.ts.
/// </summary>
public static partial class SpaFallbackExtensions
{
    private static readonly HashSet<string> StaticRoutes = new(StringComparer.OrdinalIgnoreCase)
    {
        "/", "/login", "/register", "/forgot-password", "/confirm-email", "/profile",
        "/blogs", "/blogs/new", "/exercises", "/exercise/new",
        "/contact", "/privacy", "/about", "/download",
    };

    [GeneratedRegex(@"^/(blogs/edit|exercise/edit)/\d+$", RegexOptions.IgnoreCase)]
    private static partial Regex EditRoute();

    [GeneratedRegex(@"^/blogs/(\d+)$", RegexOptions.IgnoreCase)]
    private static partial Regex BlogRoute();

    [GeneratedRegex(@"^/exercise/details/(\d+)$", RegexOptions.IgnoreCase)]
    private static partial Regex ExerciseRoute();

    public static void MapSpaFallback(this WebApplication app)
    {
        app.MapFallback(async context =>
        {
            var path = context.Request.Path.Value?.TrimEnd('/') ?? string.Empty;
            if (path.Length == 0)
            {
                path = "/";
            }

            var index = app.Environment.WebRootFileProvider.GetFileInfo("index.csr.html");
            if (!index.Exists)
            {
                index = app.Environment.WebRootFileProvider.GetFileInfo("index.html");
            }
            if (path.StartsWith("/api/", StringComparison.OrdinalIgnoreCase) || !index.Exists)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                return;
            }

            var found = await IsExistingPageAsync(path, context.RequestServices, context.RequestAborted);
            context.Response.StatusCode = found ? StatusCodes.Status200OK : StatusCodes.Status404NotFound;
            context.Response.ContentType = "text/html; charset=utf-8";
            context.Response.Headers.CacheControl = "no-cache";
            await context.Response.SendFileAsync(index, context.RequestAborted);
        });
    }

    private static async Task<bool> IsExistingPageAsync(string path, IServiceProvider services, CancellationToken token)
    {
        if (StaticRoutes.Contains(path) || EditRoute().IsMatch(path))
        {
            return true;
        }

        var blogMatch = BlogRoute().Match(path);
        if (blogMatch.Success && int.TryParse(blogMatch.Groups[1].Value, out var blogId))
        {
            var db = services.GetRequiredService<TrainingDayContext>();
            return await db.PostCultures.AnyAsync(item => item.Id == blogId, token);
        }

        var exerciseMatch = ExerciseRoute().Match(path);
        if (exerciseMatch.Success && int.TryParse(exerciseMatch.Groups[1].Value, out var codeNum))
        {
            var db = services.GetRequiredService<TrainingDayContext>();
            return await db.Exercises.AnyAsync(item => item.CodeNum == codeNum, token);
        }

        return false;
    }
}

using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace TrainingDay.Web.Server.Extensions;

public static class UrlHelperExtensions
{
    // URL of GET /api/v1/auth/confirm without the query; AccountManager appends userId and code.
    public static string EmailConfirmationUrl(this IUrlHelper urlHelper, string scheme)
    {
        var request = urlHelper.ActionContext.HttpContext.Request;
        return $"{scheme}://{request.Host}/api/v1/auth/confirm";
    }

    // Generates a link that lands on the Angular SPA reset-password page
    public static string ResetPasswordCallbackLink(this IUrlHelper urlHelper, string userId, string code, string scheme)
    {
        var request = urlHelper.ActionContext.HttpContext.Request;
        var baseUrl = $"{scheme}://{request.Host}/api/v1/auth";
        return $"{baseUrl}/reset?userId={WebUtility.UrlEncode(userId)}&code={WebUtility.UrlEncode(code)}";
    }
}
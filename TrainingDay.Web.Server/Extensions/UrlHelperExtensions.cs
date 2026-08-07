using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace TrainingDay.Web.Server.Extensions;

public static class UrlHelperExtensions
{
    // Generates a link that lands on the Angular SPA, which then calls GET /api/auth/confirm
    public static string EmailConfirmationLink(this IUrlHelper urlHelper, string userId, string code, string scheme)
    {
        var request = urlHelper.ActionContext.HttpContext.Request;
        var baseUrl = $"{scheme}://{request.Host}/api/v1/auth";
        return $"{baseUrl}/confirm?userId={WebUtility.UrlEncode(userId)}&code={WebUtility.UrlEncode(code)}";
    }

    // Generates a link that lands on the Angular SPA reset-password page
    public static string ResetPasswordCallbackLink(this IUrlHelper urlHelper, string userId, string code, string scheme)
    {
        var request = urlHelper.ActionContext.HttpContext.Request;
        var baseUrl = $"{scheme}://{request.Host}/api/v1/auth";
        return $"{baseUrl}/reset?userId={WebUtility.UrlEncode(userId)}&code={WebUtility.UrlEncode(code)}";
    }
}
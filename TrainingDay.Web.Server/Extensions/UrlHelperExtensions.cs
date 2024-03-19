using Microsoft.AspNetCore.Mvc;
using TrainingDay.Web.Server.Controllers;

namespace TrainingDay.Web.Server.Extensions;

public static class UrlHelperExtensions
{
    public static string EmailConfirmationLink(this IUrlHelper urlHelper, string userId, string code, string scheme)
    {
        return urlHelper.Action(
            action: nameof(AuthController.ConfirmEmail),
            controller: "Account",
            values: new { userId, code },
            protocol: scheme);
    }

    public static string ResetPasswordCallbackLink(this IUrlHelper urlHelper, string userId, string code, string scheme)
    {
        return urlHelper.Action(
            action: nameof(AuthController.ResetPassword),
            controller: "Account",
            values: new { userId, code },
            protocol: scheme);
    }
}
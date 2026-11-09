using Microsoft.AspNetCore.Mvc;
using TrainingDay.Web.Data.Results;

namespace TrainingDay.Web.Server.Extensions;

public static class ResultExtensions
{
    /// <summary>
    /// Returns a Problem Details response for a failed result: the status code comes from <see cref="Error.Type"/>,
    /// the title is the error description, and the error code is added as a <c>code</c> extension.
    /// </summary>
    public static ObjectResult Problem(this ControllerBase controller, Error error)
    {
        var statusCode = error.Type.ToStatusCode();
        var problem = controller.ProblemDetailsFactory.CreateProblemDetails(
            controller.HttpContext, statusCode, title: error.Description);
        problem.Extensions["code"] = error.Code;

        return new ObjectResult(problem) { StatusCode = statusCode };
    }

    /// <summary>
    /// Returns the error description as a plain-text body with the status code from <see cref="Error.Type"/>,
    /// for endpoints whose existing app versions read the message as a string instead of Problem Details.
    /// </summary>
    public static ObjectResult PlainError(this ControllerBase controller, Error error) =>
        new ObjectResult(error.Description) { StatusCode = error.Type.ToStatusCode() };

    public static int ToStatusCode(this ErrorType type) => type switch
    {
        ErrorType.Validation => StatusCodes.Status400BadRequest,
        ErrorType.NotFound => StatusCodes.Status404NotFound,
        ErrorType.Conflict => StatusCodes.Status409Conflict,
        ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
        ErrorType.Forbidden => StatusCodes.Status403Forbidden,
        _ => StatusCodes.Status500InternalServerError,
    };
}

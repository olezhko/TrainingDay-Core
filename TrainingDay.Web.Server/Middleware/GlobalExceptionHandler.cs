using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TrainingDay.Web.Data.Results;
using TrainingDay.Web.Server.Extensions;

namespace TrainingDay.Web.Server.Middleware;

/// <summary>
/// Last-resort handler for exceptions nobody caught. Expected failures are returned as a <see cref="Result"/> and never
/// reach this; whatever does is logged and answered with the same Problem Details shape as <c>this.Problem(error)</c>.
/// </summary>
internal sealed class GlobalExceptionHandler(IProblemDetailsService problemDetailsService, ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
{
    // Non-standard code nginx uses for "client closed request"; the client is gone, so nobody reads it.
    private const int ClientClosedRequest = 499;

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is OperationCanceledException && httpContext.RequestAborted.IsCancellationRequested)
        {
            httpContext.Response.StatusCode = ClientClosedRequest;
            return true;
        }

        Error error;
        int statusCode;
        if (exception is BadHttpRequestException badRequest)
        {
            // Malformed or oversized request rejected by Kestrel/model binding; it carries its own 4xx code.
            logger.LogWarning(exception, "Bad request {Method} {Path}", httpContext.Request.Method, httpContext.Request.Path);
            error = GeneralErrors.BadRequest;
            statusCode = badRequest.StatusCode;
        }
        else
        {
            logger.LogError(exception, "Unhandled exception for {Method} {Path}", httpContext.Request.Method, httpContext.Request.Path);
            error = GeneralErrors.Unexpected;
            statusCode = error.Type.ToStatusCode();
        }

        httpContext.Response.StatusCode = statusCode;
        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            // Never put exception.Message in the response: it can leak internal details.
            ProblemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = error.Description,
                Instance = httpContext.Request.Path,
                Extensions = { ["code"] = error.Code },
            },
        });
    }
}

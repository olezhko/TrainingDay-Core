namespace TrainingDay.Web.Data.Results;

/// <summary>Errors not tied to a feature, used by the web layer for unhandled failures.</summary>
public static class GeneralErrors
{
    public static readonly Error Unexpected = Error.Failure("General.Unexpected", "An unexpected error occurred.");

    public static readonly Error BadRequest = Error.Validation("General.BadRequest", "The request is invalid.");
}

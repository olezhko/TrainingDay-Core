namespace TrainingDay.Web.Data.Results;

/// <summary>Kind of failure; the web layer maps it to an HTTP status code.</summary>
public enum ErrorType
{
    Failure,
    Validation,
    NotFound,
    Conflict,
    Unauthorized,
    Forbidden,
}

using TrainingDay.Web.Data.Results;

namespace TrainingDay.Web.Services.MobileTokens;

public static class MobileTokenErrors
{
    public static readonly Error NotFound = Error.NotFound("MobileTokens.NotFound", "Device token not found.");
}

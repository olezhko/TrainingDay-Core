using TrainingDay.Common.Communication;
using TrainingDay.Web.Data.Results;
using TrainingDay.Web.Services.MobileTokens.Models;

namespace TrainingDay.Web.Services.MobileTokens;

public interface IMobileTokenManager
{
    /// <summary>Adds the device token, or updates its language, zone and last-seen time if it is already known.</summary>
    Task RegisterAsync(FirebaseTokenDto firebaseToken, CancellationToken token);

    /// <summary>Records an app action for the device. Fails with <see cref="MobileTokenErrors.NotFound"/>.</summary>
    Task<Result> RecordActionAsync(MobileActionDto action, CancellationToken token);

    /// <param name="lastActiveDays">Only tokens seen within this many days; null for all.</param>
    Task<IReadOnlyList<MobileTokenModel>> GetAllAsync(int? lastActiveDays, CancellationToken token);

    /// <summary>Fails with <see cref="MobileTokenErrors.NotFound"/>.</summary>
    Task<Result<MobileTokenModel>> GetAsync(string mobileToken, CancellationToken token);

    /// <summary>Returns the removed token. Fails with <see cref="MobileTokenErrors.NotFound"/>.</summary>
    Task<Result<MobileTokenModel>> DeleteAsync(string mobileToken, CancellationToken token);

    /// <summary>Removes tokens that never reported a last-seen time.</summary>
    Task RemoveUnusedAsync(CancellationToken token);
}

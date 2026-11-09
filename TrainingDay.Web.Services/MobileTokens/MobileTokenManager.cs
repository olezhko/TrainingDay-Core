using Microsoft.EntityFrameworkCore;
using TrainingDay.Common.Communication;
using TrainingDay.Web.Data.Results;
using TrainingDay.Web.Database;
using TrainingDay.Web.Entities;
using TrainingDay.Web.Services.Mapping;
using TrainingDay.Web.Services.MobileTokens.Models;

namespace TrainingDay.Web.Services.MobileTokens;

public class MobileTokenManager(TrainingDayContext context) : IMobileTokenManager
{
    public async Task RegisterAsync(FirebaseTokenDto firebaseToken, CancellationToken token)
    {
        var existing = await context.MobileTokens.FirstOrDefaultAsync(item => item.Token == firebaseToken.Token, token);
        if (existing != null)
        {
            existing.Language = firebaseToken.Language;
            existing.Zone = firebaseToken.Zone;
            existing.LastSend = DateTime.UtcNow;
        }
        else
        {
            context.MobileTokens.Add(new MobileToken
            {
                Token = firebaseToken.Token,
                Zone = firebaseToken.Zone,
                Language = firebaseToken.Language,
                LastSend = DateTime.UtcNow
            });
        }

        await context.SaveChangesAsync(token);
    }

    public async Task<Result> RecordActionAsync(MobileActionDto action, CancellationToken token)
    {
        var mobileToken = await context.MobileTokens.SingleOrDefaultAsync(m => m.Token == action.Token, token);
        if (mobileToken == null)
        {
            return MobileTokenErrors.NotFound;
        }

        switch (action.Action)
        {
            case MobileActions.Enter:
                mobileToken.LastSend = DateTime.UtcNow;
                break;
            case MobileActions.Workout:
                mobileToken.LastWorkoutDateTime = DateTime.UtcNow;
                break;
            case MobileActions.Weight:
                mobileToken.LastBodyControlDateTime = DateTime.UtcNow;
                break;
            default:
                break;
        }

        await context.SaveChangesAsync(token);
        return Result.Success();
    }

    public async Task<IReadOnlyList<MobileTokenModel>> GetAllAsync(int? lastActiveDays, CancellationToken token)
    {
        var query = context.MobileTokens.AsNoTracking();

        if (lastActiveDays is not null)
        {
            // Compare the bare column so an index on LastSend can be used.
            var activeSince = DateTime.UtcNow.AddDays(-lastActiveDays.Value);
            query = query.Where(item => item.LastSend > activeSince);
        }

        var tokens = await query.ToListAsync(token);
        return [.. tokens.Select(item => item.ToModel())];
    }

    public async Task<Result<MobileTokenModel>> GetAsync(string mobileToken, CancellationToken token)
    {
        var existing = await context.MobileTokens.AsNoTracking().SingleOrDefaultAsync(m => m.Token == mobileToken, token);
        return existing is null ? MobileTokenErrors.NotFound : existing.ToModel();
    }

    public async Task<Result<MobileTokenModel>> DeleteAsync(string mobileToken, CancellationToken token)
    {
        var existing = await context.MobileTokens.SingleOrDefaultAsync(m => m.Token == mobileToken, token);
        if (existing == null)
        {
            return MobileTokenErrors.NotFound;
        }

        context.MobileTokens.Remove(existing);
        await context.SaveChangesAsync(token);
        return existing.ToModel();
    }

    public async Task RemoveUnusedAsync(CancellationToken token)
    {
        // One DELETE statement instead of loading every token.
        await context.MobileTokens
            .Where(item => item.LastSend == default(DateTime))
            .ExecuteDeleteAsync(token);
    }
}

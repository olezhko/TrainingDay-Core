using TrainingDay.Common.Models;

namespace TrainingDay.Web.Services.UserRepo;

public interface IUserRepoManager
{
    Task<RepositoryBase> GetAsync(Guid userId, CancellationToken token);
    Task UpsertAsync(Guid userId, RepositoryBase data, CancellationToken token);
}

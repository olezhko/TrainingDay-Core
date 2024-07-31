using TrainingDay.Web.Data.Support;
using TrainingDay.Web.Entities;

namespace TrainingDay.Web.Data.Repositories;

public interface ISupportRepository
{
    Task Create(SupportRequest model, CancellationToken cancellationToken);
}
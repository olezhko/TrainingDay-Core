using AutoMapper;
using TrainingDay.Web.Data.Repositories;
using TrainingDay.Web.Database;
using TrainingDay.Web.Entities;

namespace TrainingDay.Web.Services.Repositories;

public class SupportRepository(TrainingDayContext context) : ISupportRepository
{
    public async Task Create(SupportRequest model, CancellationToken cancellationToken)
    {
        model.Created = DateTime.Now;
        context.SupportRequests.Add(model);

        await context.SaveChangesAsync(cancellationToken);
    }
}
namespace TrainingDay.Web.Services.Notification;

public interface IScopedProcessingService
{
    Task DoWork(CancellationToken stoppingToken);
}
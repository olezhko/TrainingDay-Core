namespace TrainingDay.Web.Services.Notification;

public interface IScopedProcessingService
{
    /// <summary>Sends the reminders whose local send time fell within (fromUtc, toUtc].</summary>
    Task DoWork(DateTime fromUtc, DateTime toUtc, CancellationToken stoppingToken);
}

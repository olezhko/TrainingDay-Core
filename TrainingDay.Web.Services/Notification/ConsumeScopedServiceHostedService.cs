using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TrainingDay.Web.Services.Notification;

public class ConsumeScopedServiceHostedService(IServiceProvider provider, ILogger<ConsumeScopedServiceHostedService> logger) : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromMinutes(1);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Each run covers the time since the previous run, so a reminder time is neither skipped nor sent twice.
        var lastRunUtc = DateTime.UtcNow;
        using var timer = new PeriodicTimer(Interval);

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            var nowUtc = DateTime.UtcNow;
            try
            {
                using var scope = provider.CreateScope();
                var scopedProcessingService = scope.ServiceProvider.GetRequiredService<IScopedProcessingService>();

                await scopedProcessingService.DoWork(lastRunUtc, nowUtc, stoppingToken);
            }
            catch (Exception e) when (e is not OperationCanceledException)
            {
                // An unhandled exception here would stop the whole host.
                logger.LogError(e, "Notification run failed");
            }

            lastRunUtc = nowUtc;
        }
    }
}

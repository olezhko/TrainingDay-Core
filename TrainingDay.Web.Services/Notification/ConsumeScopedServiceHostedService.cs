using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace TrainingDay.Web.Services.Notification;

public class ConsumeScopedServiceHostedService : BackgroundService
{
    private IServiceProvider _provider;
    public ConsumeScopedServiceHostedService(IServiceProvider provider)
    {
        _provider = provider;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await DoWork(stoppingToken);
    }

    private async Task DoWork(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _provider.CreateScope())
            {
                var scopedProcessingService = scope.ServiceProvider.GetRequiredService<IScopedProcessingService>();

                await scopedProcessingService.DoWork(stoppingToken);
            }
            //Add a delay between executions.
            await Task.Delay(2000, stoppingToken);
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        await base.StopAsync(stoppingToken);
    }
}
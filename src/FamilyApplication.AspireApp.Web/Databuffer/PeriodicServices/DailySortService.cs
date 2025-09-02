using FamilyApplication.AspireApp.Web.Databuffer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FamilyApplication.AspireApp.Web.Databuffer.PeriodicServices;
public class DailySortService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DailySortService> _logger;

    public DailySortService(IServiceProvider serviceProvider, ILogger<DailySortService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Calculate the delay until the next midnight UTC
                var now = DateTime.UtcNow;
                var nextMidnight = now.Date.AddDays(1); // Tomorrow at 00:00 UTC
                var delay = nextMidnight - now;

                _logger.LogInformation("DailySortService will run next at {NextRun}", nextMidnight);
                await Task.Delay(delay, stoppingToken); // Wait until midnight

                // Create a scope to resolve scoped services
                using (var scope = _serviceProvider.CreateScope())
                {
                    var globalVm = scope.ServiceProvider.GetRequiredService<GlobalVm>();
                    globalVm.SortBlackBoardDtos();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DailySortService");
            }
        }
    }

 
}
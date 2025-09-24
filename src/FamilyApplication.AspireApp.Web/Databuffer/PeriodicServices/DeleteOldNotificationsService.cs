namespace FamilyApplication.AspireApp.Web.Databuffer.PeriodicServices
{
    public class DeleteOldNotificationsService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DailySortService> _logger;


        public DeleteOldNotificationsService(IServiceProvider serviceProvider, ILogger<DailySortService> logger)
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
                    // Create a scope to resolve scoped services
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var globalVm = scope.ServiceProvider.GetRequiredService<GlobalVm>();
                        var userDataDto = scope.ServiceProvider.GetRequiredService<UserDtoDataService>();


                        foreach (var user in globalVm.UserDtos)
                        {
                            var oldNotifications = user.Notifications.Where(a => a.CreatedAt < DateTime.Now.AddDays(-7)).ToArray();

                            if (oldNotifications.Length != 0)
                            {
                                foreach (var notification in oldNotifications)
                                    user.Notifications.Remove(notification);

                                await userDataDto.Save(user, stoppingToken);
                                _logger.LogInformation("Deleted old notifications on user {username}", user.Username);
                            }
                        }
                    }

                    // Calculate the delay until the next midnight UTC
                    var now = DateTime.UtcNow;
                    var nextMidnight = now.Date.AddDays(1); // Tomorrow at 00:00 UTC
                    var delay = nextMidnight - now;

                    _logger.LogInformation("DailySortService will run next at {NextRun}", nextMidnight);
                    await Task.Delay(delay, stoppingToken); // Wait until midnight


                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in DailySortService");
                }
            }
        }
    }
}

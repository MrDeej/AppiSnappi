
using FamiliyApplication.AspireApp.Web.CosmosDb;
using FamiliyApplication.AspireApp.Web.CosmosDb.User;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FamiliyApplication.AspireApp.Web.Databuffer
{
    public class DataInitializer(IServiceScopeFactory scopeFactory) : IHostedService
    {

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = scopeFactory.CreateScope();
            var globalVm = scope.ServiceProvider.GetRequiredService<GlobalVm>();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var familyDtoDataService = scope.ServiceProvider.GetRequiredService<FamilyDtoDataService>();
            var userDtoDataService = scope.ServiceProvider.GetRequiredService<UserDtoDataService>();
            var blackBoardDtoDataService = scope.ServiceProvider.GetRequiredService<BlackBoardDtoDataService>();

            await familyDtoDataService.Initialize(cancellationToken);
            await userDtoDataService.Initialize(cancellationToken);
            await blackBoardDtoDataService.Initialize(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}

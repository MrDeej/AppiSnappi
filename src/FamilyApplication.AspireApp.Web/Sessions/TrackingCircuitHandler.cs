using FamilyApplication.AspireApp.Web.Databuffer;
using Microsoft.AspNetCore.Components.Server.Circuits;

namespace FamilyApplication.AspireApp.Web.Sessions
{
    public class TrackingCircuitHandler : CircuitHandler
    {
        private readonly SessionManager _sessionManager;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<TrackingCircuitHandler> _logger;

        public TrackingCircuitHandler(SessionManager sessionManager, IServiceProvider serviceProvider, IHttpContextAccessor httpContextAccessor, ILogger<TrackingCircuitHandler> logger)
        {
            _sessionManager = sessionManager;
            _serviceProvider = serviceProvider;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }


        public override Task OnConnectionUpAsync(Circuit circuit, CancellationToken cancellationToken)
        {

            try
            {
                var sessionId = circuit.Id;
                using var scope = _serviceProvider.CreateScope();

                var globalVm = scope.ServiceProvider.GetRequiredService<GlobalVm>();

                var user = _httpContextAccessor.HttpContext?.User;

                if (user?.Identity?.IsAuthenticated == true)
                {
                    var userId = user.Claims.FirstOrDefault(a=>a.Type == "emails")?.Value;
                    if (userId != null)
                    {
                        var userAccess = globalVm.UserDtos.SingleOrDefault(a => a.Username == userId);
                        if (userAccess != null)
                        {

                            _sessionManager.StartSession(sessionId, userAccess);
                        }
                    }


                    _sessionManager.RefreshSession(sessionId);


                    _logger.LogInformation("TrackingCircuitHandler.OnConnectionUpAsync Successfull {user}", userId);
                }
                else
                {
                    _sessionManager.StartSession(sessionId, null);
                    _logger.LogInformation("TrackingCircuitHandler.OnConnectionUpAsync Successfull NULL");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TrackingCircuitHandler");
            }

            return Task.CompletedTask;
        }

        public override Task OnConnectionDownAsync(Circuit circuit, CancellationToken cancellationToken)
        {

            var sessionId = circuit.Id;
            _sessionManager.EndSession(sessionId);
            _logger.LogInformation("TrackingCircuitHandler.OnConnectionDownAsync Successfull {id}", sessionId);
            return Task.CompletedTask;
        }
    }
}

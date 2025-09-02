namespace FamiliyApplication.AspireApp.Web.Sessions
{
    public class SessionCleanupService : IHostedService, IDisposable
    {
        private readonly SessionManager _sessionManager;
        private Timer? _timer;

        public SessionCleanupService(SessionManager sessionManager)
        {
            _sessionManager = sessionManager;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(CheckInactiveSessions, null, TimeSpan.Zero, TimeSpan.FromMinutes(60));
            return Task.CompletedTask;
        }

        private void CheckInactiveSessions(object? state)
        {
            var inactiveSessions = _sessionManager.ActiveSessions
                .Where(s => s.Value == null || (DateTime.UtcNow - s.Value.LastActivity) > TimeSpan.FromDays(1))
                .Select(s => s.Key)
                .ToList();

            foreach (var sessionId in inactiveSessions)
            {
                _sessionManager.EndSession(sessionId);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}

using FamilyApplication.AspireApp.Web.CosmosDb.User;
using FamilyApplication.AspireApp.Web.Databuffer;
using System.Collections.Concurrent;

namespace FamilyApplication.AspireApp.Web.Sessions
{
    public class SessionManager
    {
        private readonly ConcurrentDictionary<string, UserDto> _activeSessions = new();
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly GlobalVm _globalVm;

        public SessionManager(IHttpContextAccessor contextAccessor, GlobalVm globalVm)
        {
            _contextAccessor = contextAccessor;
            _globalVm = globalVm;
        }


        public UserDto GetMyUserDto()
        {
            var user = _contextAccessor.HttpContext?.User;
            UserDto? userAccess = null;


            if (user?.Identity?.IsAuthenticated == true)
            {
                var userId = user.FindFirst("emails")?.Value;
                if (userId != null)
                {
                    userAccess = _globalVm.UserDtos.FirstOrDefault(x => x.Username.Equals(userId, StringComparison.CurrentCultureIgnoreCase));
                }
            }

            if (userAccess != null)
                return userAccess;
            return new();

        }


        public ConcurrentDictionary<string, UserDto> ActiveSessions => _activeSessions;

        public event EventHandler? SessionChanged;

        public void StartSession(string sessionId, UserDto userAccess)
        {

            _activeSessions[sessionId] = userAccess;

            // Raise event
            OnSessionChanged();
        }

        public void EndSession(string sessionId)
        {
            if (_activeSessions.TryRemove(sessionId, out var session))
            {
                // Raise event
                OnSessionChanged();
            }
        }

        public void RefreshSession(string sessionId)
        {
            if (_activeSessions.TryGetValue(sessionId, out var session))
            {
                session.LastActivity = DateTime.UtcNow;

                // Raise event
                OnSessionChanged();
            }
        }

        protected virtual void OnSessionChanged()
        {
            SessionChanged?.Invoke(this, EventArgs.Empty);
        }
    }

}

using System.Threading.Tasks;
using FamiliyApplication.AspireApp.Web.CosmosDb.Notification;
using FamiliyApplication.AspireApp.Web.CosmosDb.User;
using FamiliyApplication.AspireApp.Web.Databuffer;
using FamiliyApplication.AspireApp.Web.Sessions;

namespace FamiliyApplication.AspireApp.Web.Components.Users
{
    public class UserNotificationManager : IDisposable
    {
        private readonly SessionManager _sessionManager;
        private readonly UserDto _currentUser;
        public int UnreadCount { get; set; }
        public event EventHandler? SomeThingChanged;

        public UserNotificationManager(SessionManager sessionManager)
        {
            _sessionManager = sessionManager;
            _currentUser = _sessionManager.GetMyUserDto();
            UpdateUnReadCount();

            _currentUser.Notifications.CollectionChanged += Notifications_CollectionChanged;
            foreach (var notification in _currentUser.Notifications)
                notification.PropertyChanged += Notification_PropertyChanged;

        }

        private bool UpdateUnReadCount()
        {
            var newUnreadCount = _currentUser.Notifications.Where(a => a.IsUnread).Count();

            if (newUnreadCount != UnreadCount)
            {
                UnreadCount = newUnreadCount;
                return true;
            }
            return false;

        }

        private void Notifications_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var changed = false;

            if (e.NewItems != null)
                foreach (var notification in e.NewItems.OfType<NotificationDto>())
                {
                    changed = true;
                    notification.PropertyChanged += Notification_PropertyChanged;
                }

            if (e.OldItems != null)
                foreach (var notification in e.OldItems.OfType<NotificationDto>())
                {
                    changed = true;
                    notification.PropertyChanged -= Notification_PropertyChanged;
                }

            if (!changed)
                return;

            UpdateUnReadCount();
            SomeThingChanged?.Invoke(sender, new EventArgs());
        }


        private Guid delayGuid;
        private async void Notification_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(NotificationDto.IsUnread))
                return;

            var tmpGuid = Guid.NewGuid();
            delayGuid = tmpGuid;
            await Task.Delay(200);
            if (delayGuid != tmpGuid)
                return;

            var changed = UpdateUnReadCount();
            if (changed)
                SomeThingChanged?.Invoke(sender, new EventArgs());

        }

        public void Dispose()
        {

            _currentUser.Notifications.CollectionChanged -= Notifications_CollectionChanged;
            foreach (var notification in _currentUser.Notifications)
                notification.PropertyChanged -= Notification_PropertyChanged;
        }
    }
}

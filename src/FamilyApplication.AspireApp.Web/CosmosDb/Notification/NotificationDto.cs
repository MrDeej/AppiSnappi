using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FamilyApplication.AspireApp.Web.CosmosDb.Notification
{
    public class NotificationDto : INotifyPropertyChanged, IEntity
    {
        [JsonProperty("id")]
        public required string Id { get; set; }

        public required string UserId { get; set; }

        public required string Title { get; set; }

        public required string Text { get; set; }

        public required NotificationDtoType NotificationDtoType { get; set; }

        public required string? ReferenceId { get; set; }

        public required string FamilyId { get; set; }

        private bool _isUnread = true;

        public bool IsUnread
        {
            get => _isUnread;
            set
            {
                if (value == _isUnread)
                    return;

                _isUnread = value;
                NotifyPropertyChanged();
            }
        }
        public required string CreatedById { get; set; }
        public required DateTime CreatedAt { get; set; } = DateTime.Now;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void NotifyPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

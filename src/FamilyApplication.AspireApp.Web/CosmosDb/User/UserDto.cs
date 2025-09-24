using FamilyApplication.AspireApp.Web.CosmosDb.Family;
using FamilyApplication.AspireApp.Web.CosmosDb.Notification;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace FamilyApplication.AspireApp.Web.CosmosDb.User
{
    public record UserDto : INotifyPropertyChanged, IEntity
    {
        private string _id = Guid.NewGuid().ToString();
        private string _username = "";
        private string _fullName = "";
        private DateOnly _birthdate;
        private UserType _userType;
        private string? _familieId;
        private string _createdBy = "";
        private DateTime _createdAt;
        private string _lastChangeBy = "";
        private DateTime _lastChangedAt;
        private string _lottieProfile = "";
        private UserWallet _wallet = new();
        private NotificationSubscription? _notificationSubscription;
        private DateTimeOffset? _lastActivity;
        private ObservableCollection<FamilyTodoDto> todosToApprove = new();
        private ObservableCollection<NotificationDto> notifications = new();
        public UserWallet Wallet
        {
            get => _wallet;
            set
            {
                if (value != _wallet)
                {
                    _wallet = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string LottieProfile
        {
            get => _lottieProfile;
            set
            {

                if (value == _lottieProfile)
                    return;

                _lottieProfile = value;
                NotifyPropertyChanged();
            }
        }

        [JsonProperty("id")]
        public string Id
        {
            get => _id;
            set
            {
                if (value == _id)
                    return;

                _id = value;
                NotifyPropertyChanged();
            }
        }



        public string Username
        {
            get => _username;
            set
            {
                if (value == _username)
                    return;

                _username = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(Surname));
            }
        }

        public string Surname
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Fullname))
                {
                    return Fullname.Split().First();
                }

                if (_username.Contains('.'))
                {
                    return _username.Split('.').First();
                }

                return _username.Split('@').First();
            }
        }

        public string Fullname
        {
            get => _fullName;
            set
            {
                if (value == _fullName)
                    return;

                _fullName = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(Surname));
                NotifyPropertyChanged(nameof(FirstName));
                NotifyPropertyChanged(nameof(Initials));
            }
        }


        public string FirstName
        {
            get
            {
                if (string.IsNullOrEmpty(Fullname))
                    return Username;

                var words = Fullname.Split();
                return words.FirstOrDefault() ?? "Missing fullname";
            }
        }

        [JsonConverter(typeof(DateOnlyJsonConverter))]
        public DateOnly BirthDate
        {
            get => _birthdate;
            set
            {
                if (value == _birthdate)
                    return;

                _birthdate = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(CurrentAge));
            }
        }


        public UserType UserType
        {
            get => _userType;
            set
            {
                if (value == _userType)
                    return;

                _userType = value;
                NotifyPropertyChanged();
            }
        }

        public string? FamilyId
        {
#pragma warning disable CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).
            get => _familieId;
#pragma warning restore CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).

            set
            {
                if (value == _familieId)
                    return;

                _familieId = value;
                NotifyPropertyChanged();
            }
        }


        public string CreatedBy
        {
            get => _createdBy;
            set
            {
                if (value == _createdBy)
                    return;

                _createdBy = value;
                NotifyPropertyChanged();
            }
        }


        public DateTime CreatedAt
        {
            get => _createdAt;
            set
            {
                if (value == _createdAt)
                    return;

                _createdAt = value;
                NotifyPropertyChanged();
            }
        }


        public string LastChangeBy
        {
            get => _lastChangeBy;
            set
            {
                if (value == _lastChangeBy)
                    return;

                _lastChangeBy = value;
                NotifyPropertyChanged();
            }
        }


        public DateTime LastChangedAt
        {
            get => _lastChangedAt;
            set
            {
                if (value == _lastChangedAt)
                    return;

                _lastChangedAt = value;
                NotifyPropertyChanged();
            }
        }

        public NotificationSubscription? NotificationSubscription
        {
            get => _notificationSubscription;
            set

            {
                if (value == _notificationSubscription)
                    return;

                _notificationSubscription = value;
                NotifyPropertyChanged();
            }
        }

        private bool? _disableNotifications;
        public bool? DisableNotifications
        {
            get => _disableNotifications;
            set
            {
                if (value != _disableNotifications)
                {
                    _disableNotifications = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public DateTimeOffset? LastActivity
        {
            get => _lastActivity;
            set
            {
                if (value != _lastActivity)
                {
                    _lastActivity = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string CurrentAge => this.GetBirthDayStringNo();

        public string Initials
        {
            get
            {
                var sb = new StringBuilder();
                foreach (var w in Fullname.Split())
                    sb.Append(w.FirstOrDefault());

                return sb.ToString(); ;
            }
        }

        public ObservableCollection<FamilyTodoDto> TodosToApprove
        {
            get
            {
                return todosToApprove;
            }
            set
            {
                if (value != todosToApprove)
                {
                    todosToApprove = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ObservableCollection<NotificationDto> Notifications
        {
            get
            {
                return notifications;
            }
            set
            {
                if(value != notifications)
                {
                    notifications = value;
                    NotifyPropertyChanged();
                }
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void NotifyPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }



      
    }
}

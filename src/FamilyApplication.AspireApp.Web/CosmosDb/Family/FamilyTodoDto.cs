using FamilyApplication.AspireApp.Web.CosmosDb.User;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FamilyApplication.AspireApp.Web.CosmosDb.Family
{
    public class FamilyTodoDto : INotifyPropertyChanged
    {
        public string Id { get; set; } = default!;


        private string _tittel = "";
        public string Tittel
        {
            get => _tittel;
            set
            {
                if (value == _tittel)
                    return;

                _tittel = value;
                OnPropertyChanged();
            }
        }



        private int? _payAmount;
        public int? PayAmount
        {
            get => _payAmount;
            set
            {
                if (value == _payAmount)
                    return;

                _payAmount = value;
                OnPropertyChanged();
            }
        }



        private FamilyTodoStatus _status;
        public FamilyTodoStatus Status
        {
            get => _status;
            set
            {
                if (value == _status)
                    return;

                _status = value;
                OnPropertyChanged();
            }
        }

        private bool _isDone;

        public bool IsDone
        {
            get => _isDone;
            set
            {
                if (value == _isDone)
                    return;

                _isDone = value;
                OnPropertyChanged();
            }
        }

        private string? _userIdBelongsTo;

        public string? UserIdBelongsTo
        {
            get => _userIdBelongsTo;
            set
            {
                if (value == _userIdBelongsTo)
                    return;

                _userIdBelongsTo = value;
                OnPropertyChanged();
            }
        }



        private string? _scheduledTodoId;
        public string? ScheduledTodoId
        {
            get => _scheduledTodoId;
            set
            {
                if (value == _scheduledTodoId)
                    return;

                _scheduledTodoId = value;
                OnPropertyChanged();
            }
        }


        private string? _description;
        public string? Description
        {
            get => _description;
            set
            {
                if (value != _description)
                {
                    _description = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public FamilyTodoDto Clone()
        {
            return new FamilyTodoDto()
            {
                Id = Id,
                Description = Description,
                IsDone = IsDone,
                PayAmount = PayAmount,
                ScheduledTodoId = ScheduledTodoId,
                Status = Status,
                Tittel = Tittel,
                //UserBelongsTo = UserBelongsTo,
                UserIdBelongsTo = UserIdBelongsTo
            };
        }
    }
}

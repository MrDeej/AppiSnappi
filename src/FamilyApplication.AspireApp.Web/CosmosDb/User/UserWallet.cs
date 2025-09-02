using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FamilyApplication.AspireApp.Web.CosmosDb.User
{
    public record UserWallet : INotifyPropertyChanged
    {

        private int _amount;

        public int Amount
        {
            get => _amount;
            set
            {
                if (value == _amount)
                    return;

                _amount = value;
                NotifyPropertyChanged();
            }
        }


        private List<UserWalletIncoming>? _incoming;

        public List<UserWalletIncoming>? Incoming
        {
            get => _incoming;
            set
            {
                if (value == _incoming)
                    return;

                _incoming = value;
                NotifyPropertyChanged();
            }
        }


        private ObservableCollection<UserWalletSaveGoal> _saveGoals = new();

        public ObservableCollection<UserWalletSaveGoal> SaveGoals
        {
            get => _saveGoals;
            set
            {
                if (value == _saveGoals)
                    return;

                _saveGoals = value;
                NotifyPropertyChanged();
            }
        }


        private DateTime _lastChangeAt;

        public DateTime LastChangedAt
        {
            get => _lastChangeAt;
            set
            {
                if (value == _lastChangeAt)
                    return;

                _lastChangeAt = value;
                NotifyPropertyChanged();
            }
        }


        private string _lastChangeBy = default!;

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

        private List<UserWalletTransactions>? _transactions;
        public List<UserWalletTransactions>? Transactions
        {
            get => _transactions;
            set
            {
                if(value != _transactions)
                {
                    _transactions = value;
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

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FamilyApplication.AspireApp.Web.CosmosDb.User
{
    public record UserWalletIncoming : INotifyPropertyChanged
    {
        private string incomingType = default!;
        public string IncomingType
        {
            get => incomingType;
            set
            {
                if (value == incomingType)
                    return;

                incomingType = value;
                NotifyPropertyChanged();
            }
        }


        private int amount;
        public int Amount
        {
            get => amount;
            set
            {
                if (value == amount)
                    return;

                amount = value;
                NotifyPropertyChanged();
            }
        }


        private string comment = default!;
        public string Comment
        {
            get => comment;
            set
            {
                if (value == comment)
                    return;

                comment = value;
                NotifyPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void NotifyPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

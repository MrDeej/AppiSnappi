using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace FamiliyApplication.AspireApp.Web.CosmosDb.User
{
    public record UserWalletSaveGoal : INotifyPropertyChanged
    {
        public string Id { get; set; }


        private string _thingToSaveFor = default!;

        public string ThingToSaveFor
        {
            get => _thingToSaveFor;
            set
            {
                if (value == _thingToSaveFor)
                    return;

                _thingToSaveFor = value;
                NotifyPropertyChanged();
            }
        }


        private string? _description = "";

        public string? Description
        {
            get => _description;
            set
            {
                if (value == _description)
                    return;

                _description = value;
                NotifyPropertyChanged();
            }
        }

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


        private DateTime _createdAt;

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

        private DateTime? finishedAt;
        public DateTime? FinishedAt
        {
            get => finishedAt;
            set
            {
                if(value != finishedAt)
                {
                    finishedAt = value;
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

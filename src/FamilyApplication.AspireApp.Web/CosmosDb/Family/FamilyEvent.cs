using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FamilyApplication.AspireApp.Web.CosmosDb.Family
{
    public class FamilyEvent : INotifyPropertyChanged
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        private DateTime date;
        public DateTime Date
        {
            get => date; 
            set
            {
                if(value != date)
                {
                    date = value;
                    OnPropertyChanged();
                }
            }
        }
        private DateTime endDate { get; set; }
        public DateTime EndDate
        {
            get => endDate;
            set
            {
                if (value != endDate)
                {
                    endDate = value;
                    OnPropertyChanged();
                }
            }
        }

        private string title { get; set; } = "";
        public string Title
        {
            get => title;
            set
            {
                if (value != title)
                {
                    title = value;
                    OnPropertyChanged();
                }
            }
        }
        private string description { get; set; } = "";
        public string Description
        {
            get => description;
            set
            {
                if (value != description)
                {
                    description = value;
                    OnPropertyChanged();
                }
            }
        }
        private string time { get; set; } = "";
        public string Time
        {
            get => time;
            set
            {
                if (value != time)
                {
                    time = value;
                    OnPropertyChanged();
                }
            }
        }
        private FamilieEventType type { get; set; }
        public FamilieEventType Type
        {
            get => type;
            set
            {
                if (value != type)
                {
                    type = value;
                    OnPropertyChanged();
                }
            }
        }

        private string tag { get; set; }
        public string Tag
        {
            get => tag;
            set
            {
                if(value != tag)
                {
                    tag = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

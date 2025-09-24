using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FamilyApplication.AspireApp.Web.CosmosDb.Family
{
    public class FamilyDto : INotifyPropertyChanged, IEntity
    {
        private string _id = Guid.NewGuid().ToString();
        private string _familyId = default!;
        private string _navn = "";
        private DateTime _opprettetTid;
        private string _opprettetAv = "";
        private int _familieKode;
        private ObservableCollection<FamilyEvent> familieEvents = new();

        [JsonProperty("id")]
        public string Id
        {
            get => _id;
            set
            {
                if (value == _id)
                    return;

                _id = value;
                OnPropertyChanged();
            }
        }

        public string FamilyId
        {
            get => _familyId;
            set
            {
                if (value == _familyId)
                    return;

                _familyId = value;
                OnPropertyChanged();
            }
        }
        public string Navn
        {
            get => _navn;
            set
            {
                if (value == _navn)
                    return;

                _navn = value;
                OnPropertyChanged();
            }
        }

        public DateTime OpprettetTid
        {
            get => _opprettetTid;
            set
            {
                if (value == _opprettetTid)
                    return;

                _opprettetTid = value;
                OnPropertyChanged();
            }
        }

        public string OpprettetAv
        {
            get => _opprettetAv;
            set
            {
                if (value == _opprettetAv)
                    return;

                _opprettetAv = value;
                OnPropertyChanged();
            }
        }

        public int FamilieKode
        {
            get => _familieKode;
            set
            {
                if (value == _familieKode)
                    return;

                _familieKode = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<FamilyEvent> FamilieEvents
        {
            get { return familieEvents; }
            set
            {
                if (familieEvents != value)
                {
                    familieEvents = value;
                    OnPropertyChanged();
                }
            }
        }

        //public ObservableCollection<FamilyTodoDto> FamilyTodos { get; } = new();

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

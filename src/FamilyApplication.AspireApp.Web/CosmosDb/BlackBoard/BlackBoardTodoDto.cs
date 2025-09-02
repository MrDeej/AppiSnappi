using FamilyApplication.AspireApp.Web.CosmosDb.Family;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FamilyApplication.AspireApp.Web.CosmosDb.BlackBoard
{
    public class BlackBoardTodoDto : INotifyPropertyChanged, IEntity
    {
        [JsonProperty("id")]
        public required string Id { get; set; }


        public required string FamilyId { get; set; }

        private FamilyTodoDto todo = default!;
        public required FamilyTodoDto Todo 
        {
            get => todo;
            set
            {
                if(value != todo)
                {
                    todo = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public List<BlackBoardTodoDtoPerformed>? ListPerformed { get; set; }


        public DateTime? CreatedAt { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void NotifyPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
    


    }

    public class BlackBoardTodoDtoPerformed
    {
        public required string Id { get; set; }
        public required string UserId { get; set; }
        public required DateTime At { get; set; }
    }
}

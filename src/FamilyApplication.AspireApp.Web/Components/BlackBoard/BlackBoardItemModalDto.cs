
using FamilyApplication.AspireApp.Web.CosmosDb.BlackBoard;
using System.Collections.ObjectModel;

namespace FamilyApplication.AspireApp.Web.Components.BlackBoard
{
    public class BlackBoardItemModalDto
    {
        public required bool IsNew { get; set; }
        public required BlackBoardTodoDto Dto { get; set; }
        public required ObservableCollection<BlackBoardItemModalDtoPerformed> ObsCollPerformed { get; set; }
    }

    public class BlackBoardItemModalDtoPerformed
    {
        public required bool IsSelected { get; set; }
        public required BlackBoardtodoDtoPerformedDto Dto { get; set; }
    }
}

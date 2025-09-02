using BlazorServerCommon.Vm;
using FamiliyApplication.AspireApp.Web.CosmosDb;
using FamiliyApplication.AspireApp.Web.CosmosDb.BlackBoard;
using FamiliyApplication.AspireApp.Web.CosmosDb.Family;
using FamiliyApplication.AspireApp.Web.CosmosDb.Notification;
using FamiliyApplication.AspireApp.Web.CosmosDb.User;
using System.Collections.ObjectModel;

namespace FamiliyApplication.AspireApp.Web.Databuffer
{
    public class GlobalVm : GlobalBase
    {
        public ObservableCollection<FamilyDto> FamilyDtos { get; } = new();
        public ObservableCollection<UserDto> UserDtos { get; } = new();
        public ObservableCollection<BlackBoardTodoDto> BlackBoardDtos { get; } = new();


        public void SortBlackBoardDtos()
        {
            var sortedList = BlackBoardDtos
        .OrderByDescending(dto => (dto.CreatedAt ?? new DateTime()) >= DateTime.UtcNow.AddDays(-7))
        .ThenByDescending(dto => (dto.ListPerformed?.Count ?? 0) > 0)
        .ThenBy(dto => dto.ListPerformed?.Count ?? 0)
        .ToList();

            BlackBoardDtos.Clear(); // Remove all existing items
            foreach (var dto in sortedList)
            {
                BlackBoardDtos.Add(dto); // Add back sorted items
            }
        }
    }
}



using FamiliyApplication.AspireApp.Web.CosmosDb.Family;

namespace FamiliyApplication.AspireApp.Web.Components.Todo
{
    public class TodoItemEditModal2Dto
    {
        public required FamilyTodoDto Todo { get; set; }
        public required bool IsNew { get; set; }
    }
}

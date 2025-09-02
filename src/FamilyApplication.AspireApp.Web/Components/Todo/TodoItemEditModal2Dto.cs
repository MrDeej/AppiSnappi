

using FamilyApplication.AspireApp.Web.CosmosDb.Family;

namespace FamilyApplication.AspireApp.Web.Components.Todo
{
    public class TodoItemEditModal2Dto
    {
        public required FamilyTodoDto Todo { get; set; }
        public required bool IsNew { get; set; }
    }
}

using FamiliyApplication.AspireApp.Web.CosmosDb.Family;

namespace FamiliyApplication.AspireApp.Web.Components.FamilyEvents
{
    public class FamilyEventDialogDto
    {
        public required FamilyEvent FamilyEvent { get; set; }
        public required bool AllowDelete { get; set; }
    }
}

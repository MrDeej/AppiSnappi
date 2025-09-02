using FamilyApplication.AspireApp.Web.CosmosDb.Family;

namespace FamilyApplication.AspireApp.Web.Components.FamilyEvents
{
    public class FamilyEventDialogDto
    {
        public required FamilyEvent FamilyEvent { get; set; }
        public required bool AllowDelete { get; set; }
    }
}

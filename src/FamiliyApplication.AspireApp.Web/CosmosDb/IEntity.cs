namespace FamiliyApplication.AspireApp.Web.CosmosDb
{
    public interface IEntity
    {
        public string Id { get; set; }
        public string FamilyId { get; set; }
    }
}

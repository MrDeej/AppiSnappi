namespace FamilyApplication.AspireApp.Web.CosmosDb.User
{
    public record UserWalletTransactions
    {
        public required string Id { get; set; }
        public required string Reason { get; set; }
        public required int Amount { get; set; }
        public required string ChangeBy { get; set; }
        public required DateTimeOffset ChangedAt { get; set; }
    }
}

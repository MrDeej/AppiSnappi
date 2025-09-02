

using FamiliyApplication.AspireApp.Web.CosmosDb.User;

namespace FamiliyApplication.AspireApp.Web.Components.UserWallet
{
    public class SaveGoalModalDto
    {
        public required UserWalletSaveGoal UserWalletSaveGoal { get; set; }
        public required bool IsNew { get; set; }
    }
}

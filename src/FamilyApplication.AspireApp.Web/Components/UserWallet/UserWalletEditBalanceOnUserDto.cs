
using FamilyApplication.AspireApp.Web.CosmosDb.User;
using System.ComponentModel.DataAnnotations;

namespace FamilyApplication.AspireApp.Web.Components.UserWallet
{
    public class UserWalletEditBalanceOnUserDto
    {
        public required int OldBalance { get; set; }

        [Range(1, 9999)]
        public int NewBalance { get; set; }

        public required UserDto User { get; set; }

        public string Reason { get; set; }
    }
}

using FamilyApplication.AspireApp.Web.CosmosDb.User;

namespace FamilyApplication.AspireApp.Web.Sessions
{
    public class SessionViewerDto
    {
        public int Count { get; set; }
        public UserDto UserDto { get; set; }
    }
}

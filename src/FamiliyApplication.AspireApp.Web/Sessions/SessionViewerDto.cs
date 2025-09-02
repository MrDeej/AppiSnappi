using FamiliyApplication.AspireApp.Web.CosmosDb.User;

namespace FamiliyApplication.AspireApp.Web.Sessions
{
    public class SessionViewerDto
    {
        public int Count { get; set; }
        public UserDto UserDto { get; set; }
    }
}

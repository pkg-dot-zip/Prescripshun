using PrescripshunLib.Models.User;
using PrescripshunLib.Models.User.Profile;

namespace PrescripshunLib.Networking.Messages
{
    public class ChattableUsersResponse : IMessage
    {
        // This message is meant to send a list of users that the client can chat with.
        public List<Guid> Users { get; set; }
        public List<IUser>
    }
}
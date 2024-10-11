using PrescripshunLib.Models.User;

namespace PrescripshunLib.Networking.Messages;
// This message is meant to send a list of users that the client can chat with.

public class ChattableUsersResponse : IMessage
{
    public List<User> Users { get; set; } = new();
}
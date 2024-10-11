namespace PrescripshunLib.Networking.Messages;

public class ChattableUsersRequest : IMessage
{
    public Guid UserKey { get; set; } = Guid.Empty;
}
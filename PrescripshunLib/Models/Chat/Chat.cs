namespace PrescripshunLib.Models.Chat;

public class Chat : IChat
{
    public Guid User1 { get; set; }
    public Guid User2 { get; set; }
    public List<IChatMessage> Messages { get; set; } = new();
}
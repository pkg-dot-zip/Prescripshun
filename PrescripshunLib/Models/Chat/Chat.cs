namespace PrescripshunLib.Models.Chat;

public class Chat
{
    public Guid User1 { get; set; }
    public Guid User2 { get; set; }
    public List<ChatMessage> Messages { get; set; } = new();
}
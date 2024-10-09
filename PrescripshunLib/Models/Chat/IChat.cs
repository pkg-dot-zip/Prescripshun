namespace PrescripshunLib.Models.Chat;

public interface IChat
{
    public Guid User1 { get; }
    public Guid User2 { get; }
    public List<IChatMessage> Messages { get; }
}
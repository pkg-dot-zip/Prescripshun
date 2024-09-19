namespace PrescripshunLib.Models.Chat
{
    public class ChatMessage : IChatMessage
    {
        public Guid Sender { get; set; }
        public Guid Recipient { get; set; }
        public string Text { get; set; } = string.Empty;
        public DateTime Time { get; set; }
    }
}

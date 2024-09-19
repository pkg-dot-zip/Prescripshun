namespace PrescripshunLib.Models.Chat
{
    public interface IChatMessage
    {
        public Guid Sender { get; }
        public Guid Recipient { get; }
        public string Text { get; }
        public DateTime Time { get; }
    }
}

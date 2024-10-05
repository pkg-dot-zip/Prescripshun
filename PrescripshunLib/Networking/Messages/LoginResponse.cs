namespace PrescripshunLib.Networking.Messages
{
    public class LoginResponse : IMessage
    {
        public Guid Id { get; set; } = Guid.Empty;
        public string Reason { get; set; } = string.Empty;

        public bool IsValid() => Id != Guid.Empty;
    }
}

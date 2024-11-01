namespace PrescripshunLib.Networking.Messages;

public class GetMedicalFileRequest : IMessage
{
    public Guid UserKey { get; set; }
}
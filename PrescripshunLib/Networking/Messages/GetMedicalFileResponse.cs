using PrescripshunLib.Models.MedicalFile;

namespace PrescripshunLib.Networking.Messages;

public class GetMedicalFileResponse : IMessage
{
    public MedicalFile MedicalFile { get; set; }
    public string Reason { get; set; }
}
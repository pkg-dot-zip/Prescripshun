namespace PrescripshunLib.Models.MedicalFile;

public class Appointment : IDescriptive
{
    public string Title { get; set; }
    public string Description { get; set; }

    public Guid DoctorToMeet { get; set; }
    public DateTime DateTime { get; set; }
}
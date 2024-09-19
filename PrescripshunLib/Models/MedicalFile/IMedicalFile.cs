namespace PrescripshunLib.Models.MedicalFile;

public interface IMedicalFile
{
    Guid Patient { get; }
    List<Diagnosis> Diagnoses { get; }
    List<Medication> Medication { get; }
    List<Appointment> Appointments { get; }
    List<Note> Notes { get; }
}
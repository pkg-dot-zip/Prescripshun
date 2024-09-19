namespace PrescripshunLib.Models.MedicalFile
{
    public class MedicalFile : IMedicalFile
    {
        public Guid Patient { get; set; }
        public List<Diagnosis> Diagnoses { get; set; } = new();
        public List<Medication> Medication { get; set; } = new();
        public List<Appointment> Appointments { get; set; } = new();
        public List<Note> Notes { get; set; } = new();
    }
}

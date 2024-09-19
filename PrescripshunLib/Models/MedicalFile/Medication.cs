namespace PrescripshunLib.Models.MedicalFile
{
    public class Medication : IDescriptive
    {
        public string Title { get; set; }
        public string Description { get; set; }

        public DateTime StartedUsingOn { get; set; }
        public DateTime? StoppedUsingOn { get; set; } = null;

        public bool IsActive() => StoppedUsingOn is not null;
    }
}

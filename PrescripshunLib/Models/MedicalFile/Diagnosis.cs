namespace PrescripshunLib.Models.MedicalFile
{
    public class Diagnosis : IDescriptive
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DateTime { get; set; }
    }
}

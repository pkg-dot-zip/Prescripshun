namespace PrescripshunLib.Models.MedicalFile;

public class Note : IDescriptive
{
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime DateTime { get; set; }
}
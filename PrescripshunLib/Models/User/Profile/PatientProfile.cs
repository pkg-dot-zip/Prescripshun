using PrescripshunLib.Models.MedicalFile;

namespace PrescripshunLib.Models.User.Profile
{
    public class PatientProfile : BaseProfile
    {
        public IMedicalFile MedicalFile { get; set; }
    }
}

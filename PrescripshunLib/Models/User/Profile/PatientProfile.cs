using PrescripshunLib.Models.MedicalFile;

namespace PrescripshunLib.Models.User.Profile
{
    public class PatientProfile : BaseProfile
    {
        public IMedicalFile MedicalFile { get; set; } // TODO: Remove! Just request the medical file from the server using the guid.
    }
}

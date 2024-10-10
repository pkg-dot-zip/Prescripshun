using Bogus;
using PrescripshunLib.Models.User.Profile;

namespace PrescripshunLib.Models.User;

public class UserPatient : BaseUser
{
    public Guid DoctоrGuid { get; set; }

    public PatientProfile GetPatientProfile => Profile as PatientProfile ?? new PatientProfile()
    {
        BirthDate = DateTime.Now,
        FullName = "Parse Error",
        ProfilePicture = new ProfilePicture(""),
    };
}
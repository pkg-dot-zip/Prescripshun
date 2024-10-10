using Bogus;
using PrescripshunLib.Models.User.Profile;

namespace PrescripshunLib.Models.User;

public class UserPatient : BaseUser
{
    public Guid DoctоrGuid { get; set; }

    public PatientProfile PatientProfile { get; set; }
}
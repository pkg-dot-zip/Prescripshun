using PrescripshunLib.Models.User.Profile;

namespace PrescripshunLib.Models.User;

public class UserDoctor : BaseUser
{
    public List<Guid> Patients { get; set; } = new();

    public DoctorProfile GetDoctorProfile => Profile as DoctorProfile ?? throw new InvalidOperationException();
}
using PrescripshunLib.Models.User.Profile;

namespace PrescripshunLib.Models.User;

public class UserDoctor : BaseUser
{
    public DoctorProfile DoctorProfile { get; set; }
}
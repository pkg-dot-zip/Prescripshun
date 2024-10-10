using Newtonsoft.Json;
using PrescripshunLib.Models.User.Profile;

namespace PrescripshunLib.Models.User;

public class UserDoctor : BaseUser
{
    public DoctorProfile GetDoctorProfile => Profile as DoctorProfile ?? new DoctorProfile()
    {
        BirthDate = DateTime.Now,
        FullName = "Parse Error",
        ProfilePicture = new ProfilePicture(""),
    };
}
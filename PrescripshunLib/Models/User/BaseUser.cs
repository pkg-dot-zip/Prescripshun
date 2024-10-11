using PrescripshunLib.Models.MedicalFile;
using PrescripshunLib.Models.User.Profile;

namespace PrescripshunLib.Models.User;

public abstract class BaseUser : IUser
{
    public Guid UserKey { get; set; }
    public required string UserName { get; set; }
    public required string Password { get; set; }
    public IProfile Profile { get; set; }
    public IMedicalFile MedicalFile { get; set; }
}
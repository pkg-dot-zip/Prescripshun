namespace PrescripshunLib.Models.User.Profile;

public interface IProfile
{
    public string FullName { get; }
    public DateTime BirthDate { get; }
    public ProfilePicture ProfilePicture { get; }
}
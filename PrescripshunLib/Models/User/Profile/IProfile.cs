namespace PrescripshunLib.Models.User.Profile;

public interface IProfile
{
    public string FullName { get; set; }
    public DateTime BirthDate { get; set; }
    public ProfilePicture ProfilePicture { get; set; }
}
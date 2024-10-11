namespace PrescripshunLib.Models.User.Profile;

public class Profile
{
    public string FullName { get; set; } = "John Doe";
    public DateTime BirthDate { get; set; } = DateTime.Now;
    public ProfilePicture ProfilePicture { get; set; } = new("https://via.placeholder.com/360x360/cccccc/9c9c9c.png");
}
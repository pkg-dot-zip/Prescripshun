namespace PrescripshunLib.Models.User.Profile
{
    public abstract class BaseProfile : IProfile
    {
        public string FullName { get; set; } = "John Doe";
        public DateTime BirthDate { get; set; } = DateTime.Now;
        public ProfilePicture? ProfilePicture { get; set; }
    }
}

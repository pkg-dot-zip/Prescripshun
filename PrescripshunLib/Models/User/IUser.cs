using PrescripshunLib.Models.User.Profile;

namespace PrescripshunLib.Models.User
{
    public interface IUser
    {
        public Guid UserKey { get; }
        public string UserName { get; }
        public string Password { get; }

        public IProfile Profile { get; }
    }
}

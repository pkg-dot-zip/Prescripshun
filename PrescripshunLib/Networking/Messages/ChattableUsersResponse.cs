using PrescripshunLib.ExtensionMethods;
using PrescripshunLib.Models.User;

namespace PrescripshunLib.Networking.Messages
{
    public class ChattableUsersResponse : IMessage
    {
        // This message is meant to send a list of users that the client can chat with.
        public List<UserPatient> Patients { get; set; } = new();
        public List<UserDoctor> Doctors { get; set; } = new();

        public List<IUser> GetChattableUsers()
        {
            List<IUser> users = new();
            users.AddAll(Patients);
            users.AddAll(Doctors);
            return users;
        }
    }
}
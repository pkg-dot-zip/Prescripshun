using PrescripshunLib.Models.Chat;
using PrescripshunLib.Models.MedicalFile;
using PrescripshunLib.Models.User;

namespace PrescripshunServer.Database
{
    internal interface IDatabase
    {
        public List<IUser> GetUsers();
        public List<UserDoctor> GetDoctors();
        public List<UserPatient> GetPatients();
        public IUser GetUser(Guid guid);
        public UserDoctor GetDoctor(Guid guid);
        public UserPatient GetPatient(Guid guid);
        public IMedicalFile GetMedicalFile(Guid guid);
        public IChat GetChat(Guid user1, Guid user2);
    }
}

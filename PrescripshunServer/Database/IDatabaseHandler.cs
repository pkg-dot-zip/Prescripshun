using PrescripshunLib.Models.Chat;
using PrescripshunLib.Models.MedicalFile;
using PrescripshunLib.Models.User;

namespace PrescripshunServer.Database
{
    internal interface IDatabaseHandler
    {
        public Task Run();

        public List<IUser> GetUsers();
        public Task AddDoctor(UserDoctor doctor);
        public List<UserDoctor> GetDoctors();
        public Task AddPatient(UserPatient patient);
        public List<UserPatient> GetPatients();
        public IUser GetUser(Guid guid);
        public UserDoctor GetDoctor(Guid guid);
        public UserPatient GetPatient(Guid guid);
        public Task AddMedicalFile(IMedicalFile medicalFile);
        public IMedicalFile GetMedicalFile(Guid guid);
        public Task AddChat(IChat chat);
        public IChat GetChat(Guid user1, Guid user2);
    }
}

using PrescripshunLib.Models.Chat;
using PrescripshunLib.Models.MedicalFile;
using PrescripshunLib.Models.User;

namespace PrescripshunServer.Database
{
    internal class DatabaseHandler : IDatabaseHandler
    {
        private readonly Database _database = new Database();

        internal async Task Run()
        {
            await _database.ConnectAsync();
            await _database.ExecuteNonQueryAsync("CREATE DATABASE MyDatabase");
            await _database.DisconnectAsync();
        }

        public List<IUser> GetUsers()
        {
            throw new NotImplementedException();
        }

        public List<UserDoctor> GetDoctors()
        {
            throw new NotImplementedException();
        }

        public List<UserPatient> GetPatients()
        {
            throw new NotImplementedException();
        }

        public IUser GetUser(Guid guid)
        {
            throw new NotImplementedException();
        }

        public UserDoctor GetDoctor(Guid guid)
        {
            throw new NotImplementedException();
        }

        public UserPatient GetPatient(Guid guid)
        {
            throw new NotImplementedException();
        }

        public IMedicalFile GetMedicalFile(Guid guid)
        {
            throw new NotImplementedException();
        }

        public IChat GetChat(Guid user1, Guid user2)
        {
            throw new NotImplementedException();
        }
    }
}

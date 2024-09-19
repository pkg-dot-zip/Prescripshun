using PrescripshunLib.Models.Chat;
using PrescripshunLib.Models.MedicalFile;
using PrescripshunLib.Models.User;

namespace PrescripshunServer.Database
{
    internal class SqlDatabaseHandler : IDatabaseHandler
    {
        private readonly SqlDatabase _sqlDatabase = new SqlDatabase();

        public async Task Run()
        {
            await _sqlDatabase.ConnectAsync();
            await _sqlDatabase.ExecuteNonQueryAsync("CREATE DATABASE MyDatabase");
            await _sqlDatabase.DisconnectAsync();
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

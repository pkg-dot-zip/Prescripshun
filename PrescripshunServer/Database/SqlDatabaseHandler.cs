using PrescripshunLib.Models.Chat;
using PrescripshunLib.Models.MedicalFile;
using PrescripshunLib.Models.User;
using SqlKata;

namespace PrescripshunServer.Database
{
    internal class SqlDatabaseHandler : IDatabaseHandler
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly SqlDatabase _sqlDatabase = new SqlDatabase();

        public async Task Run()
        {
            await _sqlDatabase.ConnectAsync();
            // await _sqlDatabase.ExecuteNonQueryAsync("CREATE DATABASE MyDatabase()");

            string tableName = "TESTTABLE";
            await _sqlDatabase.ExecuteNonQueryAsync($"CREATE TABLE IF NOT EXISTS {tableName} (\r\n  ID INTEGER PRIMARY KEY,\r\n  Title VARCHAR(30),\r\n  Description VARCHAR(30)\r\n);");
            await _sqlDatabase.ExecuteNonQueryAsync($"INSERT INTO {tableName}\r\nVALUES (483, 'mooie titel', 'mooie desc');");
            await _sqlDatabase.ExecuteNonQueryAsync($"INSERT INTO {tableName}\r\nVALUES (821, 'mooie titel', 'andere desc');");
            await _sqlDatabase.ExecuteNonQueryAsync($"INSERT INTO {tableName}\r\nVALUES (129, 'titel ding', 'desc ding');");

            _sqlDatabase.ExecuteQuery($"SELECT * FROM `{tableName}` WHERE Title = 'mooie titel';\r\n", reader =>
            {
                while (reader.Read())
                {
                    var id = reader.GetInt32("ID");
                    var title = reader.GetString("Title");
                    var desc = reader.GetString("Description");

                    Logger.Trace($"Retrieved test: {id} - {title} - {desc}");
                }
            });


            await _sqlDatabase.ExecuteNonQueryAsync($"DROP TABLE {tableName};");

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

using MySql.Data.MySqlClient;

namespace PrescripshunServer.Database
{
    internal class Database
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private const string Server = "localhost";
        private const int Port = 3306;
        private const string DbName = "prescripshundb";
        private const string Username = "root"; // 'root' by default.
        private readonly MySqlConnection _myConn = new($"Server={Server};Port={Port};Database={DbName};Uid={Username};SslMode=None"); // NOTE: Assumes there is no password.


        public void Connect()
        {
            try
            {
                _myConn.Open();
                Logger.Info("Connected to MySQL database successfully.");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Couldn't connect to database.");
                throw ex;
            }
        }

        public void Disconnect()
        {
            _myConn.Close();
            Logger.Info("Disconnected from the database.");
        }

        private void Init(MySqlConnection con)
        {
            Logger.Info("Initializing database");
            Connect();
        }

        internal void ExecuteNonQuery(MySqlConnection con, string query)
        {
            Logger.Info($"SQL NonQuery: {query}");
            var myCommand = new MySqlCommand(query, con);
            myCommand.ExecuteNonQuery();
        }

        internal void Run()
        {
            Init(_myConn);
            ExecuteNonQuery(_myConn, "CREATE DATABASE MyDatabase");
            Disconnect();
        }
    }
}

using System.Data;
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

        private void Init(MySqlConnection con)
        {
            Logger.Info("Initializing database");
            Connect();
        }
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
                throw;
            }
        }
        public bool Disconnect()
        {
            switch (_myConn.State)
            {
                case ConnectionState.Open:
                    _myConn.Close();
                    Logger.Info("Disconnected from the database.");
                    return true;
                case ConnectionState.Closed:
                    Logger.Info("Database was already closed.");
                    return false;
                default:
                    Logger.Error($"Couldn't close database connection. Current state: {_myConn.State}");
                    throw new InvalidOperationException();
            }
        }

        internal void ExecuteQuery(MySqlConnection con, string query)
        {
            Logger.Info($"Executing SQL Query: {query}");
            try
            {
                using var myCommand = new MySqlCommand(query, con);
                using var reader = myCommand.ExecuteReader();

                // Read the data returned by the query.
                while (reader.Read())
                {
                    // Example: Getting values from columns by index or by column name
                    // var id = reader.GetInt32(0); // Column 0 (Id)
                    // var name = reader.GetString("Name"); // Column "Name"
                    // var age = reader.GetInt32("Age"); // Column "Age"
                    // Logger.Info($"Patient: {id}, {name}, {age}");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "SQL Query execution failed.");
                throw;
            }
        }

        internal void ExecuteNonQuery(MySqlConnection con, string nonQuery)
        {
            Logger.Info($"Executing SQL NonQuery: {nonQuery}");
            try
            {
                var myCommand = new MySqlCommand(nonQuery, con);
                myCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "SQL NonQuery execution failed.");
                throw;
            }
        }

        internal void Run()
        {
            Init(_myConn);
            ExecuteNonQuery(_myConn, "CREATE DATABASE MyDatabase");
            Disconnect();
        }
    }
}

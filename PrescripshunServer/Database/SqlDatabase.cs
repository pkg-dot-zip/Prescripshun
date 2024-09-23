using System.Data;
using MySql.Data.MySqlClient;
using SqlKata;
using SqlKata.Compilers;

namespace PrescripshunServer.Database
{
    internal class SqlDatabase(
        string server = "localhost",
        int port = 3306,
        string dbName = "prescripshundb",
        string username = "root")
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly MySqlCompiler Compiler = new();

        private readonly MySqlConnection _myConn = new($"Server={server};Port={port};Database={dbName};Uid={username};SslMode=None"); // NOTE: Assumes there is no password.

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

        public async Task ConnectAsync()
        {
            try
            {
                await _myConn.OpenAsync();
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
        public async Task<bool> DisconnectAsync()
        {
            switch (_myConn.State)
            {
                case ConnectionState.Open:
                    await _myConn.CloseAsync();
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

        // TODO: Implement.
        internal void ExecuteQuery(Query query, Action<MySqlDataReader> action)
        {
            throw new NotImplementedException();
        }

        internal void ExecuteQuery(string query, Action<MySqlDataReader> action)
        {
            Logger.Info($"Executing SQL Query: {query}");
            try
            {
                using var myCommand = new MySqlCommand(query, _myConn);
                using var reader = myCommand.ExecuteReader();

                action.Invoke(reader);

                // Read the data returned by the query. EXAMPLE.
                // while (reader.Read())
                // {
                    // Example: Getting values from columns by index or by column name
                    // var id = reader.GetInt32(0); // Column 0 (Id)
                    // var name = reader.GetString("Name"); // Column "Name"
                    // var age = reader.GetInt32("Age"); // Column "Age"
                    // Logger.Info($"Patient: {id}, {name}, {age}");
                // }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "SQL Query execution failed.");
                throw;
            }
        }

        internal void ExecuteNonQuery(string nonQuery)
        {
            Logger.Info($"Executing SQL NonQuery: {nonQuery}");
            try
            {
                var myCommand = new MySqlCommand(nonQuery, _myConn);
                myCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "SQL NonQuery execution failed.");
                throw;
            }
        }

        internal async Task ExecuteNonQueryAsync(string nonQuery)
        {
            Logger.Info($"Executing SQL NonQuery: {nonQuery}");
            try
            {
                var myCommand = new MySqlCommand(nonQuery, _myConn);
                await myCommand.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "SQL NonQuery execution failed.");
                throw;
            }
        }
    }
}

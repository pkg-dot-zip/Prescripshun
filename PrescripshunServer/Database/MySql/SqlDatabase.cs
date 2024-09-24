using System.Data;
using MySql.Data.MySqlClient;
using SqlKata;
using SqlKata.Compilers;

namespace PrescripshunServer.Database.MySql
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="server"></param>
    /// <param name="port"></param>
    /// <param name="dbName">Name of database to connect to. NOTE: This database has to exist before launching the server as we do not create it!</param>
    /// <param name="username"></param>
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

        public void ExecuteQuery(string query, Action<MySqlDataReader> action)
        {
            Logger.Info($"Executing SQL Query: {query}");
            try
            {
                using var myCommand = new MySqlCommand(query, _myConn);
                using var reader = myCommand.ExecuteReader();

                action.Invoke(reader);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "SQL Query execution failed.");
                throw;
            }
        }

        public void ExecuteNonQuery(string nonQuery)
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

        public async Task ExecuteNonQueryAsync(string nonQuery)
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

        public bool TableExists(string tableName)
        {
            string query = $"SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = DATABASE() AND table_name = '{tableName}';";
            Logger.Info("Checking if table exists: {0}", tableName);

            try
            {
                using var myCommand = new MySqlCommand(query, _myConn);
                return Convert.ToInt32(myCommand.ExecuteScalar()) > 0;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed to check if table exists.");
                throw;
            }
        }

        public bool TableHasData(string tableName)
        {
            string query = $"SELECT COUNT(*) FROM {tableName};";
            Logger.Info("Checking if table has data: {0}", tableName);

            try
            {
                using var myCommand = new MySqlCommand(query, _myConn);
                return Convert.ToInt32(myCommand.ExecuteScalar()) > 0;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed to check if table has data.");
                throw;
            }
        }
    }
}

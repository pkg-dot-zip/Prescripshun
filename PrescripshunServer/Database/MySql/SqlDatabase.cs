using System.Data;
using MySql.Data.MySqlClient;

namespace Prescripshun.Database.MySql;

/// <summary>
/// Represents a MySQL database connection manager that handles database connection,
/// disconnection, and query execution for the Prescripshun server.
/// This class provides methods to connect to a MySQL database, execute queries, 
/// and check the existence of tables and data.
/// 
/// Note: The database specified in the constructor must already exist as this class
/// does not create it. The default configuration assumes no password is set for the MySQL connection.
/// </summary>
/// <param name="server">The server address of the MySQL database (default is 'localhost').</param>
/// <param name="port">The port number for the MySQL connection (default is 3306).</param>
/// <param name="dbName">The name of the MySQL database to connect to. This database must exist prior to launching the server.</param>
/// <param name="username">The username used to authenticate with the MySQL database (default is 'root').</param>
internal class SqlDatabase(
    string server = "localhost",
    int port = 3306,
    string dbName = "prescripshundb",
    string username = "root")
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    private readonly MySqlConnection _myConn = new($"Server={server};Port={port};Database={dbName};Uid={username};SslMode=None"); // NOTE: Assumes there is no password.

    /// <summary>
    /// Establishes a connection to the MySQL database using the provided connection parameters.
    /// Logs a success message upon a successful connection, or logs and throws an exception if the connection fails.
    /// </summary>
    /// <exception cref="MySqlException">Thrown if there is an error during the connection attempt to the database.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the connection is already open or in an invalid state.</exception>
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

    /// <summary>
    /// Asynchronously establishes a connection to the MySQL database using the provided connection parameters.
    /// Logs a success message upon a successful connection, or logs and throws an exception if the connection attempt fails.
    /// </summary>
    /// <returns>A task representing the asynchronous operation of opening the database connection.</returns>
    /// <exception cref="MySqlException">Thrown if there is an error during the asynchronous connection attempt to the database.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the connection is already open or in an invalid state.</exception>
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

    /// <summary>
    /// Disconnects from the MySQL database if the connection is currently open. 
    /// Logs a success message upon a successful disconnection, or logs if the connection was already closed.
    /// </summary>
    /// <returns>Returns <c>true</c> if the connection was open and successfully closed, 
    /// or <c>false</c> if the connection was already closed.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the connection is in an invalid state and cannot be closed.</exception>
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

    /// <summary>
    /// Asynchronously disconnects from the MySQL database if the connection is currently open. 
    /// Logs a success message upon a successful disconnection, or logs if the connection was already closed.
    /// </summary>
    /// <returns>A task representing the asynchronous operation, returning <c>true</c> if the connection was open and successfully closed, 
    /// or <c>false</c> if the connection was already closed.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the connection is in an invalid state and cannot be closed.</exception>
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

    /// <summary>
    /// Executes the provided SQL query and processes the result using the specified action.
    /// Logs the query being executed, and invokes the given action with the resulting <see cref="MySqlDataReader"/>.
    /// </summary>
    /// <param name="query">The SQL query to be executed.</param>
    /// <param name="action">The action to be performed on the <see cref="MySqlDataReader"/> containing the query results.</param>
    /// <exception cref="MySqlException">Thrown if there is an error executing the SQL query.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the connection state is invalid for executing the query.</exception>
    /// <exception cref="Exception">Thrown for any general errors during query execution.</exception>
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

    /// <summary>
    /// Executes the provided SQL non-query command, such as an INSERT, UPDATE, DELETE, or DDL statement.
    /// Logs the command being executed, and performs the operation without returning any result set.
    /// </summary>
    /// <param name="nonQuery">The SQL non-query command to be executed.</param>
    /// <exception cref="MySqlException">Thrown if there is an error executing the SQL non-query.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the connection state is invalid for executing the non-query.</exception>
    /// <exception cref="Exception">Thrown for any general errors during non-query execution.</exception>
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

    /// <summary>
    /// Asynchronously executes the provided SQL non-query command, such as an INSERT, UPDATE, DELETE, or DDL statement.
    /// Logs the command being executed, and performs the operation without returning any result set.
    /// </summary>
    /// <param name="nonQuery">The SQL non-query command to be executed.</param>
    /// <returns>A task representing the asynchronous execution of the non-query command.</returns>
    /// <exception cref="MySqlException">Thrown if there is an error executing the SQL non-query.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the connection state is invalid for executing the non-query.</exception>
    /// <exception cref="Exception">Thrown for any general errors during non-query execution.</exception>
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

    /// <summary>
    /// Checks if a table with the specified name exists in the current MySQL database.
    /// Logs the table name being checked and executes a query against the information schema to determine its existence.
    /// </summary>
    /// <param name="tableName">The name of the table to check for existence.</param>
    /// <returns>Returns <c>true</c> if the table exists, otherwise <c>false</c>.</returns>
    /// <exception cref="MySqlException">Thrown if there is an error executing the SQL query to check for the table's existence.</exception>
    /// <exception cref="Exception">Thrown for any general errors during the existence check.</exception>
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

    /// <summary>
    /// Checks if the specified table contains any data by counting the number of rows.
    /// Logs the table name being checked and executes a query to determine if any records exist.
    /// </summary>
    /// <param name="tableName">The name of the table to check for data.</param>
    /// <returns>Returns <c>true</c> if the table contains data, otherwise <c>false</c>.</returns>
    /// <exception cref="MySqlException">Thrown if there is an error executing the SQL query to check for table data.</exception>
    /// <exception cref="Exception">Thrown for any general errors during the data check.</exception>
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
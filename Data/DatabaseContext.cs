using MySqlConnector;

namespace exam.api.Data;

public class DatabaseContext
{
    private readonly string _connectionString;

    public DatabaseContext()
    {
        _connectionString = $"Server={Environment.GetEnvironmentVariable("DB_HOST")};" +
                          $"Database={Environment.GetEnvironmentVariable("DB_NAME")};" +
                          $"User={Environment.GetEnvironmentVariable("DB_USER")};" +
                          $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")};" +
                          $"Port={Environment.GetEnvironmentVariable("DB_PORT")};";
    }

    public async Task<MySqlConnection> CreateConnectionAsync()
    {
        var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();
        return connection;
    }
}
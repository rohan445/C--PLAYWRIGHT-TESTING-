using Npgsql;
using System.Threading.Tasks;

public class DbHelper
{
    private readonly string connectionString =
        "Host=localhost;Username=postgres;Password=yourpass;Database=testdb";

    public async Task<string> GetUserName(int id)
    {
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        var cmd = new NpgsqlCommand(
            "SELECT name FROM users WHERE id = @id", conn);

        cmd.Parameters.AddWithValue("id", id);

        var result = await cmd.ExecuteScalarAsync();

        return result?.ToString();
    }
}
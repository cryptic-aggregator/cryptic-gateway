using Dapper;
using Npgsql;
using GatewayService.Models;

namespace GatewayService.Repositories;

public class UserRepository
{
    private readonly string _connectionString;

    public UserRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<int> AddUserAsync(User user)
    {
        var query = "INSERT INTO gateway.\"user\" (name, password_md5, email) VALUES (@Name, @PasswordMd5, @Email) RETURNING id";

        using (var connection = new NpgsqlConnection(_connectionString))
        {
            return await connection.ExecuteScalarAsync<int>(query, new { user.Name, user.PasswordMd5, user.Email });
        }
    }

    public async Task<User> GetUserByIdAsync(int id)
    {
        var query = "SELECT * FROM gateway.\"user\" WHERE id = @Id";

        using (var connection = new NpgsqlConnection(_connectionString))
        {
            return await connection.QuerySingleOrDefaultAsync<User>(query, new {Id = id});
        }
    }

    public async Task<User> GetUserByEmailAsync(string email)
    {
        var query = "SELECT * FROM gateway.\"user\" WHERE email = @Email";

        using (var connection = new NpgsqlConnection(_connectionString))
        {
            return await connection.QuerySingleOrDefaultAsync<User>(query, new {Email = email});
        }
    }

}
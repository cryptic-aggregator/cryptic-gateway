using Cryptic_Domain.Database.Config.Interfaces;
using Cryptic_Domain.Database.Interfaces;
using Cryptic_Domain.Database.Repos.Base;
using Cryptic_Domain.Helpers;
using GatewayService.Database.Tables;
using GatewayService.Interfaces.Repositories;
using Npgsql;

namespace GatewayService.Database.Repositories;

public class UserRepository : BaseDbRepo<UserTable>, IUserRepository
{
    private readonly string _connectionString;

    public UserRepository(IDatabaseConnectionService connectionService, IDatabaseConfiguration configuration)
        : base(connectionService, configuration)
    {
    }

    public async Task<int> CreateAsync(UserTable user)
    {
        string query = $"INSERT INTO {FullTablePath} (name, password_md5, email) " +
                       "VALUES (@name, @password_md5, @email) RETURNING id;";

        using var cmd = new NpgsqlCommand(query, Connection);
        cmd.Parameters.AddWithValue("name", NpgsqlTypes.NpgsqlDbType.Text, user.Name);
        cmd.Parameters.AddWithValue("password_md5", NpgsqlTypes.NpgsqlDbType.Text, user.PasswordMd5);
        cmd.Parameters.AddWithValue("email", NpgsqlTypes.NpgsqlDbType.Text, user.Email);

        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return reader.GetInt32(0);
        }

        return 0; //TODO better to throw exception
    }

    
    public async Task<UserTable?> GetByEmailAsync(string email)
    {
        string query = $"SELECT {string.Join(", ", Columns)} FROM {FullTablePath} WHERE email = @email;";

        using var cmd = new NpgsqlCommand(query, Connection);
        cmd.Parameters.AddWithValue("email", NpgsqlTypes.NpgsqlDbType.Text, email);

        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return await reader.MapAsync<UserTable>();
        }

        return null;
    }
}
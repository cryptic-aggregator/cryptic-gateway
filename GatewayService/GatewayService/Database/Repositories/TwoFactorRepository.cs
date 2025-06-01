using Cryptic_Domain.Database.Config.Interfaces;
using Cryptic_Domain.Database.Interfaces;
using Cryptic_Domain.Database.Repos.Base;
using Cryptic_Domain.Interfaces.Database.Base;
using GatewayService.Database.Tables;
using GatewayService.Interfaces.Repositories;
using Npgsql;
using NpgsqlTypes;

namespace GatewayService.Database.Repositories;

public class TwoFactorRepository : BaseDbRepo<UserTwoFactorTable>, ITwoFactorRepository
{
    private readonly IDatabaseConfiguration _configuration;

    public TwoFactorRepository(
        IDatabaseConnectionService connectionService,
        IDatabaseConfiguration configuration)
        : base(connectionService, configuration)
    {
        _configuration = configuration;
    }

    public async Task UpsertTwoFactorSecretAsync(int userId, string secret, bool isEnabled)
    {
        // Код аналогічний тому, що був у UserRepository...
        var updateQuery = $@"
                UPDATE {FullTablePath} 
                SET secret = @secret, is_enabled = @enabled 
                WHERE user_id = @userId;
            ";
        using var updateCmd = new NpgsqlCommand(updateQuery, Connection);
        updateCmd.Parameters.AddWithValue("secret", NpgsqlDbType.Varchar, secret);
        updateCmd.Parameters.AddWithValue("enabled", NpgsqlDbType.Boolean, isEnabled);
        updateCmd.Parameters.AddWithValue("userId", NpgsqlDbType.Integer, userId);

        int rows = await updateCmd.ExecuteNonQueryAsync();
        if (rows > 0) return;

        var insertQuery = $@"
                INSERT INTO {FullTablePath} (user_id, secret, is_enabled, created_at) 
                VALUES (@userId, @secret, @enabled, NOW());
            ";
        using var insertCmd = new NpgsqlCommand(insertQuery, Connection);
        insertCmd.Parameters.AddWithValue("userId", NpgsqlDbType.Integer, userId);
        insertCmd.Parameters.AddWithValue("secret", NpgsqlDbType.Varchar, secret);
        insertCmd.Parameters.AddWithValue("enabled", NpgsqlDbType.Boolean, isEnabled);
        await insertCmd.ExecuteNonQueryAsync();
    }

    public async Task<UserTwoFactorTable?> GetTwoFactorByUserIdAsync(int userId)
    {
        var query = $@"
                SELECT id, user_id, secret, is_enabled, created_at, last_verified_at
                FROM {FullTablePath}
                WHERE user_id = @userId;
            ";
        using var cmd = new NpgsqlCommand(query, Connection);
        cmd.Parameters.AddWithValue("userId", NpgsqlDbType.Integer, userId);
        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new UserTwoFactorTable
            {
                Id = reader.GetInt32(0),
                UserId = reader.GetInt32(1),
                Secret = reader.GetString(2),
                IsEnabled = reader.GetBoolean(3),
                CreatedAt = reader.GetDateTime(4),
                LastVerifiedAt = reader.IsDBNull(5) ? (DateTime?)null : reader.GetDateTime(5)
            };
        }
        return null;
    }

    public async Task UpdateTwoFactorEnabledAsync(int userId, bool isEnabled, DateTime? lastVerifiedAt)
    {
        var query = $@"
                UPDATE {FullTablePath} 
                SET is_enabled = @enabled, last_verified_at = @lastVerifiedAt
                WHERE user_id = @userId;
            ";
        using var cmd = new NpgsqlCommand(query, Connection);
        cmd.Parameters.AddWithValue("enabled", NpgsqlDbType.Boolean, isEnabled);
        if (lastVerifiedAt.HasValue)
            cmd.Parameters.AddWithValue("lastVerifiedAt", NpgsqlDbType.TimestampTz, lastVerifiedAt.Value);
        else
            cmd.Parameters.AddWithValue("lastVerifiedAt", NpgsqlDbType.TimestampTz, DBNull.Value);

        cmd.Parameters.AddWithValue("userId", NpgsqlDbType.Integer, userId);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task DisableTwoFactorAsync(int userId)
    {
        var query = $@"
                UPDATE {FullTablePath} 
                SET is_enabled = FALSE 
                WHERE user_id = @userId;
            ";
        using var cmd = new NpgsqlCommand(query, Connection);
        cmd.Parameters.AddWithValue("userId", NpgsqlDbType.Integer, userId);
        await cmd.ExecuteNonQueryAsync();
    }
}
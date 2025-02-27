using Cryptic_Domain.Database.Config.Interfaces;
using Cryptic_Domain.Database.Interfaces;
using Cryptic_Domain.Database.Repos.Base;
using GatewayService.Database.Tables;
using GatewayService.Interfaces.Repositories;
using Npgsql;
using System.Data.Common;

namespace GatewayService.Database.Repositories;

public class RefreshTokenRepository : BaseDbRepo<RefreshTokenTable>, IRefreshTokenRepository
{
    public RefreshTokenRepository(IDatabaseConnectionService connectionService, IDatabaseConfiguration configuration)
        : base(connectionService, configuration)
    {
    }

    public async Task StoreRefreshTokenAsync(int userId, string refreshToken)
    {
        // Якщо запис для цього користувача вже існує, оновлюємо його, інакше вставляємо новий запис.
        var query = $"INSERT INTO {FullTablePath} (user_id, refresh_token) VALUES (@userId, @refreshToken)";
        using var cmd = new NpgsqlCommand(query, Connection);
        cmd.Parameters.AddWithValue("userId", NpgsqlTypes.NpgsqlDbType.Integer, userId);
        cmd.Parameters.AddWithValue("refreshToken", NpgsqlTypes.NpgsqlDbType.Varchar, refreshToken);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<string> GetRefreshTokenAsync(int userId)
    {
        var query = $"SELECT refresh_token FROM {FullTablePath} WHERE user_id = @userId;";
        using var cmd = new NpgsqlCommand(query, Connection);
        cmd.Parameters.AddWithValue("userId", NpgsqlTypes.NpgsqlDbType.Integer, userId);
        return await cmd.ExecuteScalarAsync() as string;
    }

    public async Task<int?> GetUserIdByRefreshTokenAsync(string refreshToken)
    {
        var query = $"SELECT user_id FROM {FullTablePath} WHERE refresh_token = @refreshToken;";
        using var cmd = new NpgsqlCommand(query, Connection);
        cmd.Parameters.AddWithValue("refreshToken", NpgsqlTypes.NpgsqlDbType.Varchar, refreshToken);
        var result = await cmd.ExecuteScalarAsync();
        if (result != null && int.TryParse(result.ToString(), out int userId))
        {
            return userId;
        }
        return null;
    }

    public async Task UpdateRefreshTokenAsync(int userId, string newRefreshToken)
    {
        var query = $"UPDATE {FullTablePath} SET refresh_token = @refreshToken WHERE user_id = @userId;";
        using var cmd = new NpgsqlCommand(query, Connection);
        cmd.Parameters.AddWithValue("userId", NpgsqlTypes.NpgsqlDbType.Integer, userId);
        cmd.Parameters.AddWithValue("refreshToken", NpgsqlTypes.NpgsqlDbType.Varchar, newRefreshToken);
        await cmd.ExecuteNonQueryAsync();
    }
}

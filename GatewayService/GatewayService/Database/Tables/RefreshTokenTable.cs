using NpgsqlTypes;
using Cryptic_Domain.Database.Attributes;
using Cryptic_Domain.Interfaces.Database;

namespace GatewayService.Database.Tables;

[Table("refresh_token")]
public class RefreshTokenTable : IDatabaseTable
{
    [Column("id", NpgsqlDbType.Integer)]
    public int Id { get; set; }

    [Column("user_id", NpgsqlDbType.Integer)]
    public int UserId { get; set; }

    [Column("refresh_token", NpgsqlDbType.Varchar)]
    public string RefreshToken { get; set; }
}

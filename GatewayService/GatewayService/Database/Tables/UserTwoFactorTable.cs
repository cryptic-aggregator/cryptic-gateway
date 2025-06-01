using Cryptic_Domain.Database.Attributes;
using Cryptic_Domain.Interfaces.Database;
using NpgsqlTypes;

namespace GatewayService.Database.Tables
{
    [Table("user_two_factor")]
    public class UserTwoFactorTable : IDatabaseTable
    {
        [Column("id", NpgsqlDbType.Integer)]
        public int Id { get; set; }

        [Column("user_id", NpgsqlDbType.Integer)]
        public int UserId { get; set; }

        [Column("secret", NpgsqlDbType.Varchar)]
        public string Secret { get; set; }

        [Column("is_enabled", NpgsqlDbType.Boolean)]
        public bool IsEnabled { get; set; }

        [Column("created_at", NpgsqlDbType.Timestamp)]
        public DateTime CreatedAt { get; set; }

        [Column("last_verified_at", NpgsqlDbType.Timestamp)]
        public DateTime? LastVerifiedAt { get; set; }
    }
}

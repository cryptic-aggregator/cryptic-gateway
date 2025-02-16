using Cryptic_Domain.Database.Attributes;
using Cryptic_Domain.Interfaces.Database;
using NpgsqlTypes;

namespace GatewayService.Database.Tables;

[Table("user")]
public class UserTable : IDatabaseTable
{
    [Column("id", NpgsqlDbType.Integer)]
    public int Id { get; set; }
    
    [Column("name", NpgsqlDbType.Varchar)]
    public string Name { get; set; }
    
    [Column("email", NpgsqlDbType.Varchar)]
    public string Email { get; set; }
    
    [Column("password_md5", NpgsqlDbType.Uuid)]
    public Guid PasswordMd5 { get; set; }
}
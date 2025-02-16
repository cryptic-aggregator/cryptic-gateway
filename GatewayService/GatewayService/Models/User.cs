namespace GatewayService.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public Guid PasswordMd5 { get; set; }
}

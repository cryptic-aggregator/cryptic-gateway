using System.ComponentModel.DataAnnotations;

namespace GatewayService.Models;

public class UserLoginDto
{
    [Required(ErrorMessage = "Password is required")]
    [MinLength(6, ErrorMessage = "Password must contain at least 6 characters")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email adress")]
    public string Email { get; set; }
}

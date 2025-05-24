using System.ComponentModel.DataAnnotations;

namespace GatewayService.Models.Dtos;

public class ForgotPasswordRequestDto
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; }
}

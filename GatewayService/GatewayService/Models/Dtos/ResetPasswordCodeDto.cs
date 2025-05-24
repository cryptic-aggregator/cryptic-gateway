using System.ComponentModel.DataAnnotations;

namespace GatewayService.Models.Dtos;

public class ResetPasswordCodeDto
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Reset code is required")]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "Reset code must contain 6 digit")]
    public string Code { get; set; }

    [Required(ErrorMessage = "New password is required")]
    [MinLength(6, ErrorMessage = "New password must contain at least 6 characters")]
    public string NewPassword { get; set; }
}
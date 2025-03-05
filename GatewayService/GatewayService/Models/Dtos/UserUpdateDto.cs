using System.ComponentModel.DataAnnotations;

namespace GatewayService.Models.Dtos;

public class UserUpdateDto
{
    [Required(ErrorMessage = "Ім'я є обов'язковим")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Email є обов'язковим")]
    [EmailAddress(ErrorMessage = "Некоректний формат email")]
    public string Email { get; set; }
}

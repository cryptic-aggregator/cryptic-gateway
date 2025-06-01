using System.ComponentModel.DataAnnotations;

namespace GatewayService.Models.Dtos
{
    public class TwoFactorConfirmDto
    {
        [Required]
        [StringLength(6, MinimumLength = 6)]
        public string Code { get; set; }
    }
}

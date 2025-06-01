namespace GatewayService.Models.Dtos
{
    public class TwoFactorSetupDto
    {
        public string Secret { get; set; }
        public string QrCodeBase64 { get; set; }
    }
}

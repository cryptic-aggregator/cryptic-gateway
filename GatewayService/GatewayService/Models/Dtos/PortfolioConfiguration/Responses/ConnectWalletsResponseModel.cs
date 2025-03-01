using System.Text.Json.Serialization;

namespace GatewayService.Models.Dtos.PortfolioConfiguration.Responses;

public class ConnectWalletsResponseModel
{
    [JsonPropertyName("wallets")]
    public List<WalletModel> Wallets { get; set; }
}
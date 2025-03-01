using System.Text.Json.Serialization;
using GatewayService.Models.Dtos.BlockchainInteraction.Responses;

namespace GatewayService.Models.Dtos.PortfolioConfiguration.Responses;

public class PortfolioInfoResponseModel
{
    [JsonPropertyName("portfolio")]
    public PortfolioResponseModel Portfolio { get; set; }
        
    [JsonPropertyName("wallet_info")]
    public WalletResponseModel WalletInfo { get; set; }
}
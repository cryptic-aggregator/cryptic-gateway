using System.Text.Json.Serialization;
using GatewayService.Models.Dtos.BlockchainInteraction.Responses;
using Newtonsoft.Json;

namespace GatewayService.Models.Dtos.PortfolioConfiguration.Responses;

public class PortfolioInfoResponseModel
{
    [JsonPropertyName("portfolio")]
    [JsonProperty("portfolio")]
    public PortfolioResponseModel Portfolio { get; set; }
        
    [JsonPropertyName("wallet_info")]
    [JsonProperty("wallet_info")]
    public WalletResponseModel WalletInfo { get; set; }
    
    [JsonPropertyName("balance_points")]
    [JsonProperty("balance_points")]
    public List<BalancePointDto> BalancePoints { get; set; } = new List<BalancePointDto>();
}
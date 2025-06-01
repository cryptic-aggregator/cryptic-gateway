using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace GatewayService.Models.Dtos.PortfolioConfiguration.Responses;

public class PortfolioInfoWithWalletsResponseModel
{
    [JsonPropertyName("portfolio")]
    [JsonProperty("portfolio")]
    public PortfolioResponseModel Portfolio { get; set; }

    [JsonPropertyName("walletInfo")]
    [JsonProperty("walletInfo")]
    public List<WalletWithCoinsDto> WalletInfo { get; set; } = new List<WalletWithCoinsDto>();
}
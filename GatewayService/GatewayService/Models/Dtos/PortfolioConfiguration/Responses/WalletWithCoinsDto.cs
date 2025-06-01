using System.Text.Json.Serialization;
using GatewayService.Models.Dtos.BlockchainInteraction.Responses;
using Newtonsoft.Json;

namespace GatewayService.Models.Dtos.PortfolioConfiguration.Responses;

public class WalletWithCoinsDto
{
    [JsonPropertyName("walletId")]
    [JsonProperty("walletId")]
    public int WalletId { get; set; }

    [JsonPropertyName("walletAddress")]
    [JsonProperty("walletAddress")]
    public string WalletAddress { get; set; }

    [JsonPropertyName("coins")]
    [JsonProperty("coins")]
    public List<CoinMinDto> Coins { get; set; } = new List<CoinMinDto>();
}
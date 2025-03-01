using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace GatewayService.Models.Dtos.PortfolioConfiguration.Responses;

public class WalletModel
{
    [JsonPropertyName("id")]
    [JsonProperty("id")]
    public int Id { get; set; }
        
    [JsonPropertyName("portfolio_id")]
    [JsonProperty("portfolio_id")]
    public int PortfolioId { get; set; }
        
    [JsonPropertyName("wallet_address")]
    [JsonProperty("wallet_address")]
    public string WalletAddress { get; set; }
        
    [JsonPropertyName("created_at")]
    [JsonProperty("created_at")]
    public long CreatedAt { get; set; }
}
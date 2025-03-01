using System.Text.Json.Serialization;

namespace GatewayService.Models.Dtos.PortfolioConfiguration.Responses;

public class WalletModel
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
        
    [JsonPropertyName("portfolio_id")]
    public int PortfolioId { get; set; }
        
    [JsonPropertyName("wallet_address")]
    public string WalletAddress { get; set; }
        
    [JsonPropertyName("created_at")]
    public long CreatedAt { get; set; }
}
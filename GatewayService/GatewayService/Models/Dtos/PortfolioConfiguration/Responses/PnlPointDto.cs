using System.Text.Json.Serialization;

namespace GatewayService.Models.Dtos.PortfolioConfiguration.Responses;

public class PnlPointDto
{
    [JsonPropertyName("ts")]
    public long Ts { get; set; }
    
    [JsonPropertyName("profit")] 
    public double Profit { get; set; }
    
    [JsonPropertyName("loss")] 
    public double Loss { get; set; }
}
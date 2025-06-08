using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace GatewayService.Models.Dtos.PortfolioConfiguration.Responses;

public class PortfolioCorrelationPointDto
{
    [JsonPropertyName("ts")]
    [JsonProperty("ts")]
    public long Ts { get; set; }

    [JsonPropertyName("portfolio_value")]
    [JsonProperty("portfolio_value")]
    public double PortfolioValue { get; set; }

    [JsonPropertyName("token_price")]
    [JsonProperty("token_price")]
    public double TokenPrice { get; set; }

    [JsonPropertyName("portfolio_change_pct")]
    [JsonProperty("portfolio_change_pct")]
    public double PortfolioChangePct { get; set; }

    [JsonPropertyName("token_change_pct")]
    [JsonProperty("token_change_pct")]
    public double TokenChangePct { get; set; }
}
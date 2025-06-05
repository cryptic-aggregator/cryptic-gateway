using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace GatewayService.Models.Dtos.PortfolioConfiguration.Responses;

public class TokenInfoDto
{
    [JsonPropertyName("token_id")]
    [JsonProperty("token_id")]
    public int TokenId { get; set; }

    [JsonPropertyName("symbol")]
    [JsonProperty("symbol")]
    public string Symbol { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("logo_uri")]
    [JsonProperty("logo_uri")]
    public string LogoUri { get; set; } = string.Empty;

    [JsonPropertyName("last_price")]
    [JsonProperty("last_price")]
    public string LastPrice { get; set; } = "0";
}
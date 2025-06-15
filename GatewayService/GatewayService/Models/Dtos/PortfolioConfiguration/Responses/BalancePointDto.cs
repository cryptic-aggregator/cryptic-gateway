using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace GatewayService.Models.Dtos.PortfolioConfiguration.Responses;

public class BalancePointDto
{
    [JsonPropertyName("ts")]
    [JsonProperty("ts")]
    public long Ts { get; set; }

    [JsonPropertyName("balance")]
    [JsonProperty("balance")]
    public double Balance { get; set; }
}
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace GatewayService.Models.Dtos.BlockchainInteraction.Responses;

public class CoinMinDto
{
    [JsonPropertyName("symbol")]
    [JsonProperty("symbol")]
    public string Symbol { get; set; }

    [JsonPropertyName("balance")]
    [JsonProperty("balance")]
    public string Balance { get; set; }

    [JsonPropertyName("image")]
    [JsonProperty("image")]
    public string Image { get; set; }

    [JsonPropertyName("name")]
    [JsonProperty("name")]
    public string Name { get; set; }
}
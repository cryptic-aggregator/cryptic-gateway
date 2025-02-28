using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace GatewayService.Models.Dtos.BlockchainInteraction.Responses;

public class CoinModel
{
    [JsonProperty("symbol")]
    public string Symbol { get; set; }
        
    [JsonProperty("balance")]
    public string Balance { get; set; }
}

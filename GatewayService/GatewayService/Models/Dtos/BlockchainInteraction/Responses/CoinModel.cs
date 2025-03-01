using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace GatewayService.Models.Dtos.BlockchainInteraction.Responses;

public class CoinModel
{
    [JsonProperty("symbol")]
    public string Symbol { get; set; }
        
    [JsonProperty("balance")]
    public string Balance { get; set; }
        
    [JsonProperty("avg_purchase_rice")]
    public string AvgPurchasePrice { get; set; }
        
    [JsonProperty("current_market_price")]
    public string CurrentMarketPrice { get; set; }
        
    [JsonProperty("current_value")]
    public string CurrentValue { get; set; }
}
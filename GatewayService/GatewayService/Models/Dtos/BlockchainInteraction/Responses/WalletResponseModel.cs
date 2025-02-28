using Newtonsoft.Json;

namespace GatewayService.Models.Dtos.BlockchainInteraction.Responses;

public class WalletResponseModel
{
    [JsonProperty("coins")]
    public List<CoinModel> Coins { get; set; }
}
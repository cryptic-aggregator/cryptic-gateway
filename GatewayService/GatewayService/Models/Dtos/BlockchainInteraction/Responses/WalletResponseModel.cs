using Newtonsoft.Json;

namespace GatewayService.Models.Dtos.BlockchainInteraction.Responses;

public class WalletResponseModel
{
    [JsonProperty("coins")]
    public List<CoinModel> Coins { get; set; }
        
    [JsonProperty("total_portfolio_value_USDT")]
    public string TotalPortfolioValueUSDT { get; set; }
}
namespace GatewayService.Models.Dtos.PortfolioConfiguration.Responses;

public class PortfolioCalculationResponseModel
{
    public PortfolioResponseModel Portfolio { get; set; }
    public List<WalletCoinResultDto> CalculatedCoins { get; set; }
}

public class WalletCoinResultDto
{
    public string Symbol { get; set; }
    public string Image { get; set; }
    public string DollarValue { get; set; }
    public string Percentage { get; set; }
}
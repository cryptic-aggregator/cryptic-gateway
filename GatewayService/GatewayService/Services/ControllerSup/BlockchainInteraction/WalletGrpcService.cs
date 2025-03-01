using Cryptic.BlockchainInteraction.Models.Requests;
using Cryptic.BlockchainInteraction.Rpc;
using GatewayService.Interfaces.Services;
using GatewayService.Models.Dtos.BlockchainInteraction.Responses;

namespace GatewayService.Services.ControllerSup.BlockchainInteraction;

public class WalletGrpcService : IWalletGrpcService
{
    private readonly WalletService.WalletServiceClient _grpcClient;

    public WalletGrpcService(WalletService.WalletServiceClient grpcClient)
    {
        _grpcClient = grpcClient;
    }

    public async Task<WalletResponseModel> GetWalletCoinsAsync(List<string> addresses)
    {
        var grpcRequest = new GetWalletCoinsRequest();
        grpcRequest.Address.Add(addresses);
            
        var grpcResponse = await _grpcClient.GetWalletCoinsAsync(grpcRequest);
            
        var coins = new List<CoinModel>();
        foreach (var coin in grpcResponse.Coins)
        {
            coins.Add(new CoinModel
            {
                Symbol = coin.Symbol,
                Balance = coin.Balance,
                AvgPurchasePrice = coin.AvgPurchasePrice,
                CurrentMarketPrice = coin.CurrentMarketPrice,
                CurrentValue = coin.CurrentValue
            });
        }
            
        return new WalletResponseModel 
        { 
            Coins = coins,
            TotalPortfolioValueUSDT = grpcResponse.TotalPortfolioValueUSDT
        };
    }
}
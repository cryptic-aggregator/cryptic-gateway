using Cryptic_Domain.Enums.Portfolio;
using Cryptic.PortfolioConfiguration.Models.Requests;
using Cryptic.PortfolioConfiguration.Models.Responses;
using Cryptic.PortfolioConfiguration.Rpc;
using GatewayService.Interfaces.Services;
using GatewayService.Models.Dtos.BlockchainInteraction.Responses;
using GatewayService.Models.Dtos.PortfolioConfiguration.Requests;
using GatewayService.Models.Dtos.PortfolioConfiguration.Responses;

namespace GatewayService.Services.ControllerSup.PortfolioConfiguration;

public class PortfolioGrpcService : IPortfolioGrpcService
{
    private readonly PortfolioService.PortfolioServiceClient _grpcClient;

    public PortfolioGrpcService(PortfolioService.PortfolioServiceClient grpcClient)
    {
        _grpcClient = grpcClient;
    }

    public async Task<PortfolioResponseModel> CreatePortfolioAsync(CreatePortfolioRequestModel request, int ownerId)
    {
        var grpcRequest = new CreatePortfolioRequest
        {
            Name = request.Name,
            OwnerId = ownerId,
        };

        var grpcResponse = await _grpcClient.CreatePortfolioAsync(grpcRequest);
        var portfolio = grpcResponse.Portfolio;
        return new PortfolioResponseModel
        {
            Id = portfolio.Id,
            Name = portfolio.Name,
            OwnerId = portfolio.OwnerId,
            CreatedAt = portfolio.CreatedAt
        };
    }

    public async Task<PortfolioResponseModel> GetPortfolioAsync(int id, int ownerId)
    {
        var grpcRequest = new GetPortfolioRequest
        {
            Id = id,
            OwnerId = ownerId
        };

        var grpcResponse = await _grpcClient.GetPortfolioAsync(grpcRequest);
        var portfolio = grpcResponse.Portfolio;
        return new PortfolioResponseModel
        {
            Id = portfolio.Id,
            Name = portfolio.Name,
            OwnerId = portfolio.OwnerId,
            CreatedAt = portfolio.CreatedAt
        };
    }

    public async Task<List<PortfolioResponseModel>> GetPortfoliosByOwnerAsync(int ownerId)
    {
        var grpcRequest = new GetPortfoliosByOwnerRequest { OwnerId = ownerId };
        var grpcResponse = await _grpcClient.GetPortfoliosByOwnerAsync(grpcRequest);
        return grpcResponse.Portfolios.Select(p => new PortfolioResponseModel
        {
            Id = p.Id,
            Name = p.Name,
            OwnerId = p.OwnerId,
            CreatedAt = p.CreatedAt
        }).ToList();
    }

    public async Task<PortfolioResponseModel> UpdatePortfolioAsync(UpdatePortfolioRequestModel portfolio, int id,
        int ownerId)
    {
        var grpcRequest = new UpdatePortfolioRequest
        {
            Portfolio = new Portfolio
            {
                Id = id,
                Name = portfolio.Name,
                OwnerId = ownerId,
            }
        };

        var grpcResponse = await _grpcClient.UpdatePortfolioAsync(grpcRequest);
        var updatedPortfolio = grpcResponse.Portfolio;
        return new PortfolioResponseModel
        {
            Id = updatedPortfolio.Id,
            Name = updatedPortfolio.Name,
            OwnerId = updatedPortfolio.OwnerId,
            CreatedAt = updatedPortfolio.CreatedAt
        };
    }

    public async Task<bool> DeletePortfolioAsync(int id, int ownerId)
    {
        var grpcRequest = new DeletePortfolioRequest
        {
            Id = id,
            OwnerId = ownerId
        };

        var grpcResponse = await _grpcClient.DeletePortfolioAsync(grpcRequest);
        return grpcResponse.Result.Success;
    }

    public async Task<ConnectWalletsResponseModel> ConnectWalletsAsync(
        ConnectWalletsRequestModel request,
        int portfolioId,
        int ownerId)
    {
        // Створюємо gRPC-запит і додаємо кожен WalletConnectEntity
        var grpcRequest = new ConnectWalletsRequest
        {
            PortfolioId = portfolioId,
            OwnerId = ownerId
        };

        foreach (var w in request.Wallets)
        {
            grpcRequest.Wallets.Add(new WalletConnectEntity
            {
                Name = w.Name,
                CaipAddress = w.CaipAddress,
                Connector = w.Connector,
                ConnectionType = (int)w.ConnectionType,
                WalletAddress = w.WalletAddress
            });
        }
        
        var grpcResponse = await _grpcClient.ConnectWalletsAsync(grpcRequest);
        
        return new ConnectWalletsResponseModel
        {
            Wallets = grpcResponse.Wallets
                .Select(w => new WalletModel
                {
                    Id = w.Id,
                    PortfolioId = w.PortfolioId,
                    Name = w.Name,
                    CaipAddress = w.CaipAddress,
                    Connector = w.Connector,
                    ConnectionType = (WalletConnectionType)w.ConnectionType,
                    Visibility = (WalletVisibility)w.Visibility,
                    WalletAddress = w.WalletAddress,
                    CreatedAt = w.CreatedAt
                })
                .ToList()
        };
    }

    public async Task<PortfolioInfoResponseModel> GetPortfolioInfoAsync(int id, int ownerId)
    {
        var grpcRequest = new GetPortfolioInfoRequest()
        {
            PortfolioId = id,
            OwnerId = ownerId
        };

        var grpcResponse = await _grpcClient.GetPortfolioInfoAsync(grpcRequest);

        var portfolioDto = new PortfolioResponseModel
        {
            Id = grpcResponse.Portfolio.Id,
            Name = grpcResponse.Portfolio.Name,
            OwnerId = grpcResponse.Portfolio.OwnerId,
            CreatedAt = grpcResponse.Portfolio.CreatedAt
        };

        var walletDto = new WalletResponseModel()
        {
            Coins = grpcResponse.WalletInfo.Coins.Select(c => new CoinModel
            {
                Symbol = c.Symbol,
                Balance = c.Balance,
                AvgPurchasePrice = c.AvgPurchasePrice,
                CurrentMarketPrice = c.CurrentMarketPrice,
                CurrentValue = c.CurrentValue,
                PriceChange1hPercent = c.PriceChange1HPercent,
                ChangeSinceAvgPurchase = c.ChangeSinceAvgPurchase,
                Image = c.Image,
                Name = c.Name,
            }).ToList(),
            TotalPortfolioValueUSDT = grpcResponse.WalletInfo.TotalPortfolioValueUSDT
        };

        return new PortfolioInfoResponseModel
        {
            Portfolio = portfolioDto,
            WalletInfo = walletDto
        };
    }

    public async Task<List<WalletModel>> GetWalletsByPortfolioIdAsync(int portfolioId)
    {
        var request = new GetWalletsByPortfolioIdRequest
        {
            PortfolioId = portfolioId
        };

        var response = await _grpcClient.GetWalletsByPortfolioIdAsync(request);

        return response.Wallets.Select(w => new WalletModel
        {
            Id = w.Id,
            PortfolioId = w.PortfolioId,
            WalletAddress = w.WalletAddress,
            CreatedAt = w.CreatedAt,
            ConnectionType = (WalletConnectionType)w.ConnectionType,
            Visibility = (WalletVisibility)w.Visibility,
        }).ToList();
    }

    public async Task<bool> PatchWalletVisibility(int portfolioId, int walletId, int visibility)
    {
        var request = new PatchWalletVisibilityRequest
        {
            PortfolioId = portfolioId,
            WalletId = walletId,
            Visibility = visibility
        };

        var response = await _grpcClient.PatchWalletVisibilityAsync(request);
        return response.Result.Success;
    }

    public async Task<PortfolioCalculationResponseModel> GetPortfolioCalculationAsync(int portfolioId, int ownerId)
    {
        var request = new GetPortfolioCalculationRequest
        {
            PortfolioId = portfolioId
        };

        var grpcResponse = await _grpcClient.GetPortfolioCalculationAsync(request);

        var result = new PortfolioCalculationResponseModel
        {
            Portfolio = new PortfolioResponseModel
            {
                Id = grpcResponse.Portfolio.Id,
                Name = grpcResponse.Portfolio.Name,
                OwnerId = grpcResponse.Portfolio.OwnerId,
                CreatedAt = grpcResponse.Portfolio.CreatedAt
            },
            CalculatedCoins = grpcResponse.CalculatedCoins
                .Select(coin => new WalletCoinResultDto
                {
                    Symbol = coin.Symbol,
                    Image = coin.Image,
                    DollarValue = coin.DollarValue,
                    Percentage = coin.Percentage
                })
                .ToList(),
        };

        return result;
    }
}
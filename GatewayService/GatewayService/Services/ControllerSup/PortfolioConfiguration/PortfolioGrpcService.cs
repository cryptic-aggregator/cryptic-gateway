using Cryptic.PortfolioConfiguration.Models.Requests;
using Cryptic.PortfolioConfiguration.Models.Responses;
using Cryptic.PortfolioConfiguration.Rpc;
using GatewayService.Interfaces.Services;
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

    public async Task<PortfolioResponseModel> CreatePortfolioAsync(CreatePortfolioRequestModel request)
    {
        var grpcRequest = new CreatePortfolioRequest
        {
            Name = request.Name,
            OwnerId = 1,
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

    public async Task<PortfolioResponseModel> UpdatePortfolioAsync(UpdatePortfolioRequestModel portfolio, int id, int ownerId)
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
}

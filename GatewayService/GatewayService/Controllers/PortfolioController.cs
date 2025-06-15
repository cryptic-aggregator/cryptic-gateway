using System.Security.Claims;
using GatewayService.Controllers.Base;
using GatewayService.Interfaces.Services;
using GatewayService.Models.Dtos.BlockchainInteraction.Requests;
using GatewayService.Models.Dtos.PortfolioConfiguration.Requests;
using GatewayService.Models.Dtos.PortfolioConfiguration.Responses;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GatewayService.Controllers;

[ApiController]
[Route("api/portfolio")]
public class PortfolioController : BaseController
{
    //TODO change userId = 1 to user id from UserClaims
    private readonly IPortfolioGrpcService _portfolioGrpcService;

    public PortfolioController(IPortfolioGrpcService portfolioGrpcService)
    {
        _portfolioGrpcService = portfolioGrpcService;
    }

    [HttpPost]
    public async Task<IActionResult> CreatePortfolio([FromBody] CreatePortfolioRequestModel request)
    {
        var result = await _portfolioGrpcService.CreatePortfolioAsync(request, UserClaims.UserId);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPortfolio(int id)
    {
        var result = await _portfolioGrpcService.GetPortfolioAsync(id, UserClaims.UserId);
        if (result == null)
            return NotFound();
        return Ok(result);
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetPortfoliosByOwner()
    {
        var result = await _portfolioGrpcService.GetPortfoliosByOwnerAsync(UserClaims.UserId);
        return Ok(result);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdatePortfolio([FromBody] UpdatePortfolioRequestModel portfolio, int id)
    {
        var result = await _portfolioGrpcService.UpdatePortfolioAsync(portfolio, id, UserClaims.UserId);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePortfolio(int id)
    {
        var success = await _portfolioGrpcService.DeletePortfolioAsync(id, UserClaims.UserId);
        if (success)
            return Ok();
        return BadRequest();
    }

    [HttpPost("{id}/connect-wallets")]
    public async Task<IActionResult> ConnectWallets([FromBody] ConnectWalletsRequestModel request, int id)
    {
        var result = await _portfolioGrpcService.ConnectWalletsAsync(request, id, UserClaims.UserId);
        return Ok(result);
    }

    [HttpGet("{id}/info")]
    public async Task<IActionResult> GetPortfolioInfo(int id)
    {
        var result = await _portfolioGrpcService.GetPortfolioInfoAsync(id, UserClaims.UserId);
        return Ok(result);
    }

    [HttpPatch("{portfolioId}/wallet/{walletId}")]
    public async Task<IActionResult> PatchWalletVisibility(
        int portfolioId,
        int walletId,
        [FromBody] PatchWalletVisibilityModel model)
    {
        if (model == null)
            return BadRequest("No body provided");

        var success = await _portfolioGrpcService.PatchWalletVisibility(portfolioId, walletId, (int)model.Visibility);

        if (!success)
            return NotFound("Wallet not found or no rows were updated.");

        return Ok();
    }

    [HttpGet("{id}/wallets")]
    public async Task<IActionResult> GetAllWallets(
        int id,
        [FromQuery] WalletsFilterModel filters)
    {
        if (id <= 0)
            return BadRequest("Invalid portfolio ID");

        var wallets = await _portfolioGrpcService.GetWalletsByPortfolioIdAsync(id, filters);
        return Ok(wallets);
    }

    [HttpGet("{id}/analytic/allocations")]
    public async Task<IActionResult> GetPortfolioCalculation(int id)
    {
        var ownerId = UserClaims.UserId;

        var result = await _portfolioGrpcService.GetPortfolioCalculationAsync(id, ownerId);
        return Ok(result);
    }

    [HttpGet("{id}/transactions")]
    public async Task<ActionResult<PortfolioTransactionsResponseModel>> GetPortfolioTransactions(
        int id,
        [FromQuery] GetPortfolioTransactionsRequestModel filters)
    {
        var ownerId = UserClaims.UserId;
        if (id <= 0)
            return BadRequest("Invalid portfolio ID");

        PortfolioTransactionsResponseModel result;
        try
        {
            result = await _portfolioGrpcService.GetPortfolioTransactionsAsync(id, ownerId, filters);
        }
        catch (Grpc.Core.RpcException ex) when (ex.StatusCode == Grpc.Core.StatusCode.NotFound)
        {
            return NotFound(ex.Status.Detail);
        }

        return Ok(result);
    }
    
    [HttpDelete("{portfolioId}/wallet/{walletId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteWallet(int portfolioId, int walletId)
    {
        if (portfolioId <= 0 || walletId <= 0)
            return BadRequest("Invalid portfolio or wallet ID");

        var ownerId = UserClaims.UserId;

        bool success;
        try
        {
            success = await _portfolioGrpcService.DeleteWalletAsync(portfolioId, walletId, ownerId);
        }
        catch (Grpc.Core.RpcException ex) when (ex.StatusCode == Grpc.Core.StatusCode.NotFound)
        {
            return NotFound(ex.Status.Detail);
        }

        if (!success)
            return BadRequest("Failed to delete wallet");

        return NoContent();
    }

    [HttpGet("{id}/correlation")]
    public async Task<ActionResult<List<PortfolioCorrelationResponseModel>>> GetCorrelation(
        int id,
        [FromQuery] PortfolioCorrelationRequestModel filters)
    {
        var ownerId = UserClaims.UserId;
        if (id <= 0)
            return BadRequest("Invalid portfolio ID");

        PortfolioCorrelationResponseModel result;
        try
        {
            result = await _portfolioGrpcService
                .GetPortfolioCorrelationAsync(id, ownerId, filters);
        }
        catch (Grpc.Core.RpcException ex)
            when (ex.StatusCode == Grpc.Core.StatusCode.NotFound)
        {
            return NotFound(ex.Status.Detail);
        }

        return Ok(result);
    }

    [HttpGet("{id}/info-with-wallets")]
    public async Task<IActionResult> GetPortfolioInfoWithWallets(int id)
    {
        var ownerId = UserClaims.UserId;
        var result = await _portfolioGrpcService.GetPortfolioInfoWithWalletsAsync(id, ownerId);
        return Ok(result);
    }
    
    [HttpGet("{id}/pnl")]
    public async Task<ActionResult<PortfolioPnlResponseModel>> GetPortfolioPnl(
        int id,
        [FromQuery] long fromTs,
        [FromQuery] long toTs,
        [FromQuery] int pointsCount = 12)
    {
        var ownerId = UserClaims.UserId;
        if (id <= 0 || fromTs >= toTs)
            return BadRequest("Invalid parameters");

        PortfolioPnlResponseModel result;
        try
        {
            result = await _portfolioGrpcService.GetPortfolioPnlPointsAsync(
                id, ownerId, fromTs, toTs, pointsCount);
        }
        catch (RpcException ex) when (ex.StatusCode == Grpc.Core.StatusCode.NotFound)
        {
            return NotFound(ex.Status.Detail);
        }

        return Ok(result);
    }
}
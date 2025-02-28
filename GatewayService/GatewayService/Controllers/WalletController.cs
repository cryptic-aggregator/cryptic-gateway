using GatewayService.Interfaces.Services;
using GatewayService.Models.Dtos.BlockchainInteraction.Requests;
using Microsoft.AspNetCore.Mvc;

namespace GatewayService.Controllers;

[ApiController]
[Route("api/wallet")]
public class WalletController : ControllerBase
{
    private readonly IWalletGrpcService _walletGrpcService;

    public WalletController(IWalletGrpcService walletGrpcService)
    {
        _walletGrpcService = walletGrpcService;
    }

    [HttpPost("coins")]
    public async Task<IActionResult> GetWalletCoins([FromBody] WalletRequestModel request)
    {
        if (!request.Address.Any())
        {
            return BadRequest("Address is required!");
        }

        var result = await _walletGrpcService.GetWalletCoinsAsync(request.Address);
        return Ok(result);
    }
}
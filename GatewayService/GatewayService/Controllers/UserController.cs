using GatewayService.Models;
using GatewayService.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using GatewayService.Controllers.Base;
using Microsoft.AspNetCore.Authorization;

namespace GatewayService.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : BaseController
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] UserRegisterDto userDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = await _userService.RegisterUserAsync(userDto);

        if (userId == 0)
            return BadRequest("Registration failed");

        return Ok(new { UserId = userId });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);

        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto loginDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var tokenResponse = await _userService.LoginAsync(loginDto);
        if (tokenResponse == null)
            return Unauthorized(new { message = "Invalid email or password" });

        return Ok(tokenResponse);
    }

    [Authorize]
    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteAccount()
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            return Unauthorized();

        var result = await _userService.DeleteAccountAsync(userId);
        if (!result)
            return NotFound(new { message = "User not found" });

        return Ok(new { message = "Account deleted successfully" });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
    {
        var tokenResponse = await _userService.RefreshTokenAsync(refreshToken);
        if (tokenResponse == null)
            return Unauthorized(new { message = "Invalid refresh token" });

        return Ok(tokenResponse);
    }

    [Authorize]
    [HttpGet("profile")]
    public IActionResult GetProfile()
    {
        if (HttpContext.Items["UserClaims"] is UserClaims userClaims)
        {
            // Використовуємо дані з userClaims
            return Ok(new
            {
                UserId = userClaims.UserId,
                Email = userClaims.Email,
                Name = userClaims.Name
            });
        }
        return Unauthorized();
    }
}
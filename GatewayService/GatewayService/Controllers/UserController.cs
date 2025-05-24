using GatewayService.Models;
using GatewayService.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using GatewayService.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using GatewayService.Models.Dtos;

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
    
    [AllowAnonymous]
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

    [AllowAnonymous]
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
    public async Task<IActionResult> GetProfile()
    {
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (idClaim == null || !int.TryParse(idClaim, out var userId))
            return Unauthorized();

        var userDto = await _userService.GetUserByIdAsync(userId);
        if (userDto == null)
            return NotFound();

        return Ok(new
        {
            userId = userId,
            Email = userDto.Email,
            Name = userDto.Name,
        });
    }

    [Authorize]
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UserUpdateDto updateDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Отримуємо user id з даних, збережених middleware (UserClaims)
        if (UserClaims == null)
            return Unauthorized();

        var success = await _userService.UpdateUserProfileAsync(UserClaims.UserId, updateDto);
        if (success)
            return Ok(new { message = "Профіль оновлено успішно" });
        else
            return BadRequest(new { message = "Оновлення профілю не вдалося" });
    }

    [HttpPost("forgot-password-code")]
    public async Task<IActionResult> ForgotPasswordCode([FromBody] ForgotPasswordRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _userService.RequestPasswordResetCodeAsync(request);
        return Ok(new { message = "Якщо email зареєстрований, вам надіслано код для скидання паролю." });
    }

    [HttpPost("reset-password-code")]
    public async Task<IActionResult> ResetPasswordCode([FromBody] ResetPasswordCodeDto resetDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        bool result = await _userService.ResetPasswordWithCodeAsync(resetDto);
        if (result)
            return Ok(new { message = "Пароль успішно скинуто." });
        else
            return BadRequest(new { message = "Невірний код або email." });
    }
}
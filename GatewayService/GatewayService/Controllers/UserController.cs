using GatewayService.Models;
using GatewayService.Services;
using Microsoft.AspNetCore.Mvc;

namespace GatewayService.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] UserRegisterDto userDto)
    {
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
    public async Task<IActionResult> GetUserByEmail([FromBody] LoginDto loginDto)
    {
        var token = await _userService.LoginAsync(loginDto);
        if (token == null)
            return Unauthorized(new {message = "Invalid email or password"});

        return Ok(token);
    }
}
using Microsoft.AspNetCore.Mvc;
using ProjectFlow.DTOs;
using ProjectFlow.Services;

namespace ProjectFlow.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto model)
    {
        var result = await _authService.RegisterAsync(model);
        
        if (!result.Success)
            return BadRequest(new { message = result.Message });

        return Ok(new { message = "Usuario registrado exitosamente", userId = result.UserId });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto model)
    {
        var result = await _authService.LoginAsync(model);
        
        if (!result.Success)
            return Unauthorized(new { message = result.Message });

        return Ok(new 
        { 
            token = result.Token,
            refreshToken = result.RefreshToken,
            expiration = result.Expiration,
            user = new 
            {
                result.UserId,
                result.Email,
                result.Roles
            }
        });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto model)
    {
        var result = await _authService.RefreshTokenAsync(model.Token, model.RefreshToken);
        
        if (!result.Success)
            return Unauthorized(new { message = result.Message });

        return Ok(new 
        { 
            token = result.Token,
            refreshToken = result.RefreshToken,
            expiration = result.Expiration
        });
    }
}

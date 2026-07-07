using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectFlow.Data;
using ProjectFlow.DTOs;
using ProjectFlow.Models;
using ProjectFlow.Security;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using TaskModel = ProjectFlow.Models.Task;

namespace ProjectFlow.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly JwtSettings _jwtSettings;

    public AuthService(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        IOptions<JwtSettings> jwtSettings)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<AuthResultDto> RegisterAsync(RegisterDto model)
    {
        var existingUser = await _userManager.FindByEmailAsync(model.Email);
        if (existingUser != null)
            return new AuthResultDto { Success = false, Message = "El email ya está registrado" };

        var user = new IdentityUser
        {
            UserName = model.Email,
            Email = model.Email,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
            return new AuthResultDto 
            { 
                Success = false, 
                Message = string.Join(", ", result.Errors.Select(e => e.Description)) 
            };

        // Asignar rol Member por defecto
        await _userManager.AddToRoleAsync(user, "Member");

        return new AuthResultDto 
        { 
            Success = true, 
            Message = "Usuario registrado exitosamente",
            UserId = user.Id,
            Email = user.Email
        };
    }

    public async Task<AuthResultDto> LoginAsync(LoginDto model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
            return new AuthResultDto { Success = false, Message = "Credenciales inválidas" };

        var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
        if (!result.Succeeded)
            return new AuthResultDto { Success = false, Message = "Credenciales inválidas" };

        var roles = await _userManager.GetRolesAsync(user);
        var token = GenerateJwtToken(user, roles);
        var refreshToken = GenerateRefreshToken();

        return new AuthResultDto
        {
            Success = true,
            Token = token,
            RefreshToken = refreshToken,
            Expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
            UserId = user.Id,
            Email = user.Email,
            Roles = roles
        };
    }

    public async Task<AuthResultDto> RefreshTokenAsync(string token, string refreshToken)
    {
        // Implementación básica - en producción validar el refresh token almacenado
        var principal = GetPrincipalFromExpiredToken(token);
        if (principal == null)
            return new AuthResultDto { Success = false, Message = "Token inválido" };

        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return new AuthResultDto { Success = false, Message = "Token inválido" };

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return new AuthResultDto { Success = false, Message = "Usuario no encontrado" };

        var roles = await _userManager.GetRolesAsync(user);
        var newToken = GenerateJwtToken(user, roles);
        var newRefreshToken = GenerateRefreshToken();

        return new AuthResultDto
        {
            Success = true,
            Token = newToken,
            RefreshToken = newRefreshToken,
            Expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes)
        };
    }

    private string GenerateJwtToken(IdentityUser user, IList<string> roles)
    {
        var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
            SigningCredentials = credentials,
            Issuer = _jwtSettings.ValidIssuer,
            Audience = _jwtSettings.ValidAudience
        };

        var handler = new JwtSecurityTokenHandler();
        var securityToken = handler.CreateToken(tokenDescriptor);
        return handler.WriteToken(securityToken);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = false,
            ValidIssuer = _jwtSettings.ValidIssuer,
            ValidAudience = _jwtSettings.ValidAudience,
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };

        var handler = new JwtSecurityTokenHandler();
        try
        {
            var principal = handler.ValidateToken(token, tokenValidationParameters, out _);
            return principal;
        }
        catch
        {
            return null;
        }
    }
}

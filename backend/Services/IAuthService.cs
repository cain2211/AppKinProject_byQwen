using ProjectFlow.DTOs;

namespace ProjectFlow.Services;

public interface IAuthService
{
    Task<AuthResultDto> RegisterAsync(RegisterDto model);
    Task<AuthResultDto> LoginAsync(LoginDto model);
    Task<AuthResultDto> RefreshTokenAsync(string token, string refreshToken);
}

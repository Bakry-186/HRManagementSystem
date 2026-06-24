using HRM.Application.DTOs.Auth;

namespace HRM.Application.Interfaces;

public interface IAuthService
{
    Task<RegisterResultDto> RegisterAsync(RegisterDto dto);
    Task<string?> LoginAsync(LoginDto dto);
}

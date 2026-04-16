using AiWorkbench.Application.DTOs.Auth;

namespace AiWorkbench.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken ct = default);
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request, CancellationToken ct = default);
    Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request, CancellationToken ct = default);
    Task LogoutAsync(RefreshTokenRequestDto request, CancellationToken ct = default);
    Task LogoutAllAsync(Guid userId, CancellationToken ct = default);
    Task<List<SessionDto>> GetSessionsAsync(Guid userId, CancellationToken ct = default);
    Task<bool> RevokeSessionAsync(Guid userId, string token, CancellationToken ct);
}
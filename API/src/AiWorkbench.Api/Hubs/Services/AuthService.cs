using AiWorkbench.Api.Services;
using AiWorkbench.Application.DTOs.Auth;
using AiWorkbench.Application.Interfaces;
using AiWorkbench.Application.Repositories;
using AiWorkbench.Domain.Entities;
using Microsoft.AspNetCore.SignalR;

namespace AiWorkbench.Api.Hubs.Services;

public class AuthService(IUserRepository users, IRefreshTokenRepository refreshTokens, IJwtTokenGenerator jwt, IUserConnectionService connections, IHubContext<SessionHub> hub) : IAuthService
{
    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken ct = default)
    {
        var existing = await users.GetByEmailAsync(request.Email, ct);
        if (existing is not null)
            throw new InvalidOperationException("Email already registered.");

        var user = new User
        {
            DisplayName = request.Name,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = "User"
        };

        await users.AddAsync(user, ct);

        var accessToken = jwt.GenerateToken(user);
        var refreshToken = CreateRefreshToken(user.Id);

        await refreshTokens.AddAsync(refreshToken, ct);

        return new AuthResponseDto
        {
            Token = accessToken,
            RefreshToken = refreshToken.Token,
            Email = user.Email,
            Role = user.Role
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request, CancellationToken ct = default)
    {
        var user = await users.GetByEmailAsync(request.Email, ct);
        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials.");

        var accessToken = jwt.GenerateToken(user);
        var refreshToken = CreateRefreshToken(user.Id);

        await refreshTokens.AddAsync(refreshToken, ct);

        return new AuthResponseDto
        {
            Token = accessToken,
            RefreshToken = refreshToken.Token,
            Email = user.Email,
            Role = user.Role
        };
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request, CancellationToken ct = default)
    {
        var existing = await refreshTokens.GetByTokenAsync(request.RefreshToken, ct);

        if (existing is null || existing.IsExpired || existing.IsRevoked || existing.ReplacedByToken != null)
        {
            throw new UnauthorizedAccessException("Invalid refresh token.");
        }

        existing.RevokedAt = DateTime.Now;

        var newRefresh = CreateRefreshToken(existing.UserId);
        existing.ReplacedByToken = newRefresh.Token;

        await refreshTokens.UpdateAsync(existing, ct);
        await refreshTokens.AddAsync(newRefresh, ct);

        var user = await users.GetByIdAsync(existing.UserId, ct) ?? throw new UnauthorizedAccessException("User not found.");
        var accessToken = jwt.GenerateToken(user);

        return new AuthResponseDto
        {
            Token = accessToken,
            RefreshToken = newRefresh.Token,
            Email = user.Email,
            Role = user.Role
        };
    }

    public async Task LogoutAsync(RefreshTokenRequestDto request, CancellationToken ct = default)
    {
        var token = await refreshTokens.GetByTokenAsync(request.RefreshToken, ct);
        if (token is null || token.IsRevoked) return;

        token.RevokedAt = DateTime.Now;
        await refreshTokens.UpdateAsync(token, ct);

        var userConnections = connections.GetConnections(token.UserId);

        foreach (var connectionId in userConnections)
        {
            await hub.Clients.Client(connectionId).SendAsync("logout", ct);
        }
    }

    public async Task LogoutAllAsync(Guid userId, CancellationToken ct = default)
    {
        await refreshTokens.RevokeAllForUserAsync(userId, ct);

        var userConnections = connections.GetConnections(userId);

        foreach (var connectionId in userConnections)
        {
            await hub.Clients.Client(connectionId).SendAsync("logout", ct);
        }
    }

    private static RefreshToken CreateRefreshToken(Guid userId)
    {
        return new RefreshToken
        {
            UserId = userId,
            Token = Convert.ToBase64String(Guid.NewGuid().ToByteArray()) + Guid.NewGuid(),
            ExpiresAt = DateTime.Now.AddDays(7)
        };
    }

    public async Task<List<SessionDto>> GetSessionsAsync(Guid userId, CancellationToken ct = default)
    {
        var tokens = await refreshTokens.GetAllForUserAsync(userId, ct);

        return [.. tokens
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new SessionDto
            {
                RefreshToken = t.Token,
                CreatedAt = t.CreatedAt,
                ExpiresAt = t.ExpiresAt,
                IsActive = t.IsActive
            })];
    }
}

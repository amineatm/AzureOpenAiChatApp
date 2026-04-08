using AiWorkbench.Application.DTOs.Auth;
using AiWorkbench.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AiWorkbench.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterRequestDto dto, CancellationToken ct)
    {
        var register = await authService.RegisterAsync(dto, ct);
        return Ok(register);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginRequestDto dto, CancellationToken ct)
    {
        var login = await authService.LoginAsync(dto, ct);

        return Ok(login);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh(RefreshTokenRequestDto dto, CancellationToken ct)
    {
        var refresh = await authService.RefreshTokenAsync(dto, ct);
        return Ok(refresh);
    }

    [HttpPost("logout")]
    [AllowAnonymous]
    public async Task<IActionResult> Logout(RefreshTokenRequestDto dto, CancellationToken ct)
    {
        await authService.LogoutAsync(dto, ct);
        return Ok();
    }

    [HttpPost("logout-all")]
    [Authorize]
    public async Task<IActionResult> LogoutAll(CancellationToken ct)
    {
        var userId = User.FindFirst("sub")?.Value;
        if (userId is null) return Unauthorized();

        await authService.LogoutAllAsync(Guid.Parse(userId), ct);
        return Ok();
    }

    [HttpGet("sessions")]
    [Authorize]
    public async Task<IActionResult> GetSessions(CancellationToken ct)
    {
        var userId = User.FindFirst("sub")?.Value;
        if (userId is null) return Unauthorized();

        var sessions = await authService.GetSessionsAsync(Guid.Parse(userId), ct);
        return Ok(sessions);
    }

    [HttpDelete("sessions/{token}")]
    [Authorize]
    public async Task<IActionResult> RevokeSession(string token, CancellationToken ct)
    {
        var userId = User.FindFirst("sub")?.Value;
        if (userId is null) return Unauthorized();

        await authService.LogoutAsync(new RefreshTokenRequestDto { RefreshToken = token }, ct);
        return Ok();
    }

}
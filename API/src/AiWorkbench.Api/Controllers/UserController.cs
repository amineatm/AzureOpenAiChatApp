using AiWorkbench.Application.DTOs.User;
using AiWorkbench.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AiWorkbench.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UserController(IUserService userService) : ControllerBase
{
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser(CancellationToken ct)
    {
        var userIdStr = User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
        {
            return Unauthorized();
        }

        try
        {
            var user = await userService.GetUserAsync(userId, ct);
            return Ok(user);
        }
        catch (KeyNotFoundException)
        {
            return NotFound("User not found.");
        }
    }

    [HttpPut("me")]
    [Authorize]
    public async Task<IActionResult> UpdateCurrentUser([FromBody] UpdateUserRequestDto dto, CancellationToken ct)
    {
        var userIdStr = User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
        {
            return Unauthorized();
        }

        var updatedUser = await userService.UpdateUserAsync(userId, dto, ct);
        return Ok(updatedUser);

    }
}

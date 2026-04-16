using AiWorkbench.Application.DTOs.User;

namespace AiWorkbench.Application.Interfaces;

public interface IUserService
{
    Task<UserResponseDto> GetUserAsync(Guid userId, CancellationToken ct = default);
    Task<UserResponseDto> UpdateUserAsync(Guid userId, UpdateUserRequestDto request, CancellationToken ct = default);
}

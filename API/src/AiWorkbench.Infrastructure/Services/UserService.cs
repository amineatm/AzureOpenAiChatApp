using AiWorkbench.Application.DTOs.User;
using AiWorkbench.Application.Interfaces;
using AiWorkbench.Application.Repositories;

namespace AiWorkbench.Infrastructure.Services;

public class UserService(IUserRepository userRepository) : IUserService
{
    public async Task<UserResponseDto> GetUserAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await userRepository.GetByIdAsync(userId, ct);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {userId} not found.");
        }

        return new UserResponseDto
        {
            Id = user.Id,
            Email = user.Email,
            DisplayName = user.DisplayName,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<UserResponseDto> UpdateUserAsync(Guid userId, UpdateUserRequestDto request, CancellationToken ct = default)
    {
        var user = await userRepository.GetByIdAsync(userId, ct);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {userId} not found.");
        }

        if (!string.IsNullOrWhiteSpace(request.DisplayName))
        {
            user.DisplayName = request.DisplayName;
        }

        await userRepository.UpdateAsync(user, ct);

        return new UserResponseDto
        {
            Id = user.Id,
            Email = user.Email,
            DisplayName = user.DisplayName,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        };
    }
}

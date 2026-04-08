using AiWorkbench.Domain.Entities;

namespace AiWorkbench.Application.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}
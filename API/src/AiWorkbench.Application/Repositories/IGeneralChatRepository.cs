using AiWorkbench.Domain.Entities;

namespace AiWorkbench.Application.Repositories
{
    public interface IGeneralChatRepository
    {
        Task<GeneralChatMessage> AddAsync(GeneralChatMessage message, CancellationToken ct = default);
        Task<GeneralChatMessage?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<IReadOnlyList<GeneralChatMessage>> GetRecentAsync(int limit = 50, CancellationToken ct = default);
        Task DeleteAsync(Guid id, CancellationToken ct = default);
        Task<List<GeneralChatMessage>> GetByUserAsync(Guid userId, CancellationToken ct = default);

    }
}
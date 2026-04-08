using AiWorkbench.Domain.Entities;

namespace AiWorkbench.Application.Repositories
{
    public interface ISpeechSessionRepository
    {
        Task AddAsync(SpeechSession session, CancellationToken ct = default); 
        Task<List<SpeechSession>> GetUserSessionsAsync(Guid userId, CancellationToken ct = default);
    }
}

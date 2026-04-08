using AiWorkbench.Application.DTOs.RAG;
using AiWorkbench.Domain.Entities;

namespace AiWorkbench.Application.Interfaces
{
    public interface IRagService
    {
        Task<RagQueryResponseDto> QueryAsync(RagQueryRequestDto request, CancellationToken ct = default);
        Task<List<RagChatMessage>> GetHistoryAsync(Guid userId, CancellationToken ct);
    }

}

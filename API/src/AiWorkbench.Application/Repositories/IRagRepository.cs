using AiWorkbench.Domain.Entities;

namespace AiWorkbench.Application.Repositories
{
    public interface IRagRepository
    {
        Task AddManyEmbeddingsAsync(IEnumerable<DocumentEmbedding> embeddings, CancellationToken ct = default);
        Task<List<DocumentEmbedding>> GetEmbeddingsByDocumentIdAsync(Guid documentId, int limit = 50, CancellationToken ct = default);
        Task<RagChatMessage> AddAsync(RagChatMessage message, CancellationToken ct = default);
        Task<RagChatMessage?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task DeleteAsync(Guid id, CancellationToken ct = default);
        Task<List<RagChatMessage>> GetHistoryAsync(Guid userId, CancellationToken ct);
    }
}

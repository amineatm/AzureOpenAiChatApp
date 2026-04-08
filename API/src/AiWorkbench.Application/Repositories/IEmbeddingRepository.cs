using AiWorkbench.Domain.Entities;

namespace AiWorkbench.Application.Repositories
{
    public interface IEmbeddingRepository
    {
        Task<DocumentEmbedding> AddAsync(DocumentEmbedding embedding, CancellationToken ct = default);
        Task<DocumentEmbedding?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<IReadOnlyList<DocumentEmbedding>> GetBySourceDocumentIdAsync(Guid documentId, CancellationToken ct = default);
        Task DeleteByDocumentIdAsync(Guid documentId, CancellationToken ct = default);
    }
}
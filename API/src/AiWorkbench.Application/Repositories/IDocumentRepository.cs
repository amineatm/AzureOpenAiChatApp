using AiWorkbench.Domain.Entities;

namespace AiWorkbench.Application.Repositories
{
    public interface IDocumentRepository
    {
        Task<UploadedDocument> AddAsync(UploadedDocument doc, CancellationToken ct = default);
        Task<UploadedDocument?> GetLatestForUserAsync(Guid userId, CancellationToken ct);
        Task<IReadOnlyList<UploadedDocument>> ListAsync(int skip = 0, int take = 50, CancellationToken ct = default);
        Task UpdateAsync(UploadedDocument doc, CancellationToken ct = default);
        Task DeleteAsync(UploadedDocument doc, CancellationToken ct = default);
        Task<UploadedDocument?> GetByIdAsync(Guid id, CancellationToken ct = default);
    }
}
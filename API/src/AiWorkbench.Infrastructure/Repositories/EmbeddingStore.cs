using AiWorkbench.Application.Repositories;
using AiWorkbench.Domain.Entities;
using AiWorkbench.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AiWorkbench.Infrastructure.Repositories
{
    public class EmbeddingStore(AiWorkbenchDbContext context) : IEmbeddingRepository
    {

        public async Task<DocumentEmbedding> AddAsync(DocumentEmbedding embedding, CancellationToken ct = default)
        {
            await context.DocumentEmbeddings.AddAsync(embedding, ct);
            await context.SaveChangesAsync(ct);
            return embedding;
        }

        public async Task<DocumentEmbedding?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
            await context.DocumentEmbeddings.FindAsync([id], ct).AsTask();

        public async Task<IReadOnlyList<DocumentEmbedding>> GetBySourceDocumentIdAsync(Guid documentId, CancellationToken ct = default) =>
            await context.DocumentEmbeddings
                     .AsNoTracking()
                     .Where(e => e.DocumentId == documentId)
                     .ToListAsync(ct);

        public async Task DeleteByDocumentIdAsync(Guid documentId, CancellationToken ct = default)
        {
            var items = await context.DocumentEmbeddings.Where(e => e.DocumentId == documentId).ToListAsync(ct);
            if (!items.Any()) return;
            context.DocumentEmbeddings.RemoveRange(items);
            await context.SaveChangesAsync(ct);
        }
    }
}
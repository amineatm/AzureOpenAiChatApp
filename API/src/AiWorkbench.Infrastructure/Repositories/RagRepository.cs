using AiWorkbench.Application.Repositories;
using AiWorkbench.Domain.Entities;
using AiWorkbench.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AiWorkbench.Infrastructure.Repositories
{
    public class RagRepository(AiWorkbenchDbContext context) : IRagRepository
    {
        public async Task AddManyEmbeddingsAsync(IEnumerable<DocumentEmbedding> embeddings, CancellationToken ct = default)
        {
            await context.DocumentEmbeddings.AddRangeAsync(embeddings, ct);
            await context.SaveChangesAsync(ct);
        }

        public async Task<List<DocumentEmbedding>> GetEmbeddingsByDocumentIdAsync(Guid documentId, int limit = 50, CancellationToken ct = default)
        {
            return await context.DocumentEmbeddings
                .Where(e => e.DocumentId == documentId)
                .OrderBy(e => e.ChunkIndex)
                .Take(limit)
                .ToListAsync(ct);
        }

        public async Task<RagChatMessage> AddAsync(RagChatMessage message, CancellationToken ct = default)
        {
            await context.RagChatMessages.AddAsync(message, ct);
            await context.SaveChangesAsync(ct);
            return message;
        }

        public async Task<RagChatMessage?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await context.RagChatMessages.FindAsync([id], ct).AsTask();
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            var entity = await context.RagChatMessages.FindAsync([id], ct).AsTask();
            if (entity is null) return;

            context.RagChatMessages.Remove(entity);
            await context.SaveChangesAsync(ct);
        }
        public async Task<List<RagChatMessage>> GetHistoryAsync(Guid userId, CancellationToken ct)
        {
            var query = context.RagChatMessages
                .AsNoTracking()
                .Where(x => x.UserId == userId);

            return await query
                .OrderBy(x => x.CreatedAt)
                .ToListAsync(ct);
        }

    }

}
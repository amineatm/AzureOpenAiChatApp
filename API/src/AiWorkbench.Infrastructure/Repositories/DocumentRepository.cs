using AiWorkbench.Application.Repositories;
using AiWorkbench.Domain.Entities;
using AiWorkbench.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AiWorkbench.Infrastructure.Repositories
{
    public class DocumentRepository(AiWorkbenchDbContext context) : IDocumentRepository
    {

        public async Task<UploadedDocument> AddAsync(UploadedDocument doc, CancellationToken ct = default)
        {
            await context.UploadedDocuments.AddAsync(doc, ct);
            await context.SaveChangesAsync(ct);
            return doc;
        }
        public async Task<UploadedDocument?> GetLatestForUserAsync(Guid userId, CancellationToken ct)
        {
            return await context.UploadedDocuments
                .Where(d => d.UserId == userId)
                .OrderByDescending(d => d.UploadedAt)
                .FirstOrDefaultAsync(ct);
        }


        public async Task<IReadOnlyList<UploadedDocument>> ListAsync(int skip = 0, int take = 50, CancellationToken ct = default)
        {
            return await context.UploadedDocuments
                     .AsNoTracking()
                     .Skip(skip)
                     .Take(take)
                     .ToListAsync(ct);
        }

        public async Task UpdateAsync(UploadedDocument doc, CancellationToken ct = default)
        {
            context.UploadedDocuments.Update(doc);
            await context.SaveChangesAsync(ct);
        }

        public Task DeleteAsync(UploadedDocument doc, CancellationToken ct)
        {
            context.UploadedDocuments.Remove(doc);
            return context.SaveChangesAsync(ct);
        }

        public async Task<UploadedDocument?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await context.UploadedDocuments
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id, ct);
        }



    }
}
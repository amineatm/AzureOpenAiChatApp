using AiWorkbench.Application.Repositories;
using AiWorkbench.Domain.Entities;
using AiWorkbench.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AiWorkbench.Infrastructure.Repositories
{
    public class GeneralChatRepository(AiWorkbenchDbContext context) : IGeneralChatRepository
    {

        public async Task<GeneralChatMessage> AddAsync(GeneralChatMessage message, CancellationToken ct = default)
        {
            await context.GeneralChatMessages.AddAsync(message, ct);
            await context.SaveChangesAsync(ct);
            return message;
        }

        public async Task<GeneralChatMessage?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
            await context.GeneralChatMessages.FindAsync([id], ct).AsTask();

        public async Task<IReadOnlyList<GeneralChatMessage>> GetRecentAsync(int limit = 50, CancellationToken ct = default) =>
            await context.GeneralChatMessages
                     .AsNoTracking()
                     .OrderByDescending(m => m.CreatedAt)
                     .Take(limit)
                     .ToListAsync(ct);

        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            var entity = await context.GeneralChatMessages.FindAsync([id], ct).AsTask();
            if (entity is null) return;
            context.GeneralChatMessages.Remove(entity);
            await context.SaveChangesAsync(ct);
        }

        public async Task<List<GeneralChatMessage>> GetByUserAsync(Guid userId, CancellationToken ct = default)
        {
            return await context.GeneralChatMessages
                .Where(x => x.UserId == userId)
                .OrderBy(x => x.CreatedAt)
                .ToListAsync(ct);
        }

    }
}
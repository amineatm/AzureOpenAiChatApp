using AiWorkbench.Application.Repositories;
using AiWorkbench.Domain.Entities;
using AiWorkbench.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AiWorkbench.Infrastructure.Repositories;

public class SpeechSessionRepository(AiWorkbenchDbContext db) : ISpeechSessionRepository
{
    public async Task AddAsync(SpeechSession session, CancellationToken ct = default)
    {
        db.SpeechSessions.Add(session);
        await db.SaveChangesAsync(ct);
    }
    public async Task<List<SpeechSession>> GetUserSessionsAsync(Guid userId, CancellationToken ct = default)
    {
        return await db.SpeechSessions
            .Where(x => x.UserId == userId)
            .OrderBy(x => x.StartedAt)
            .ToListAsync(ct);
    }
}

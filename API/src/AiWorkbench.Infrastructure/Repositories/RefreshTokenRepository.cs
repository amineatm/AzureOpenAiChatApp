using AiWorkbench.Application.Repositories;
using AiWorkbench.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AiWorkbench.Infrastructure.Repositories
{
    public class RefreshTokenRepository(AiWorkbenchDbContext context) : IRefreshTokenRepository
    {
        public Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct = default)
        {
            return context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == token, ct);
        }

        public async Task AddAsync(RefreshToken token, CancellationToken ct = default)
        {
            context.RefreshTokens.Add(token);
            await context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(RefreshToken token, CancellationToken ct = default)
        {
            context.RefreshTokens.Update(token);
            await context.SaveChangesAsync(ct);
        }
        public Task<List<RefreshToken>> GetAllForUserAsync(Guid userId, CancellationToken ct = default)
        {
            return context.RefreshTokens
                .Where(x => x.UserId == userId)
                .ToListAsync(ct);
        }

        public async Task RevokeAllForUserAsync(Guid userId, CancellationToken ct = default)
        {
            await context.RefreshTokens
                .Where(x => x.UserId == userId && x.RevokedAt == null && x.ExpiresAt > DateTime.Now)
                .ExecuteUpdateAsync(setters => setters.SetProperty(t => t.RevokedAt, DateTime.Now), ct);
        }
    }
}

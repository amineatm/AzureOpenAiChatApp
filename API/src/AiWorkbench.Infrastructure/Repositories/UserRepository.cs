using AiWorkbench.Application.Repositories;
using AiWorkbench.Domain.Entities;
using AiWorkbench.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AiWorkbench.Infrastructure.Repositories
{
    public class UserRepository(AiWorkbenchDbContext context) : IUserRepository
    {
        public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
        {
            return context.Users.FirstOrDefaultAsync(x => x.Email == email, ct);
        }

        public Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return context.Users.FirstOrDefaultAsync(x => x.Id == id, ct);
        }

        public async Task AddAsync(User user, CancellationToken ct = default)
        {
            context.Users.Add(user);
            await context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(User user, CancellationToken ct = default)
        {
            context.Users.Update(user);
            await context.SaveChangesAsync(ct);
        }
    }
}

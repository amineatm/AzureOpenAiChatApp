using AiWorkbench.Application.Repositories;
using AiWorkbench.Domain.Entities;
using AiWorkbench.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AiWorkbench.Infrastructure.Repositories
{
    public class ImageRepository(AiWorkbenchDbContext context) : IImageRepository
    {
        public async Task<ImageGeneration> AddAsync(ImageGeneration image, CancellationToken ct = default)
        {
            await context.ImageGenerations.AddAsync(image, ct);
            await context.SaveChangesAsync(ct);
            return image;
        }

        public async Task<ImageGeneration?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
            await context.ImageGenerations.FindAsync([id], ct).AsTask();

        public async Task<IReadOnlyList<ImageGeneration>> ListByUserAsync(Guid userId, int skip = 0, int take = 50, CancellationToken ct = default) =>
            await context.ImageGenerations
                     .AsNoTracking()
                     .Where(i => i.UserId == userId)
                     .OrderByDescending(i => i.CreatedAt)
                     .Skip(skip)
                     .Take(take)
                     .ToListAsync(ct);

        public async Task UpdateAsync(ImageGeneration image, CancellationToken ct = default)
        {
            context.ImageGenerations.Update(image);
            await context.SaveChangesAsync(ct);
        }
    }
}
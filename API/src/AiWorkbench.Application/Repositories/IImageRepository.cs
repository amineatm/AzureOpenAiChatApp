using AiWorkbench.Domain.Entities;

namespace AiWorkbench.Application.Repositories
{
    public interface IImageRepository
    {
        Task<ImageGeneration> AddAsync(ImageGeneration image, CancellationToken ct = default);
        Task<ImageGeneration?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<IReadOnlyList<ImageGeneration>> ListByUserAsync(Guid userId, int skip = 0, int take = 50, CancellationToken ct = default);
        Task UpdateAsync(ImageGeneration image, CancellationToken ct = default);
    }
}

using AiWorkbench.Application.DTOs.Images;
using AiWorkbench.Domain.Entities;

namespace AiWorkbench.Application.Interfaces
{
    public interface IImageService
    {
        Task<ImageGenerationResponseDto> GenerateImageAsync(ImageGenerationRequestDto request, CancellationToken ct = default);
        Task<IReadOnlyList<ImageGeneration>> ListByUserAsync(Guid userId, int skip = 0, int take = 50, CancellationToken ct = default);
    }

}

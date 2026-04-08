using AiWorkbench.Application.DTOs.Images;
using AiWorkbench.Application.Interfaces;
using AiWorkbench.Application.Repositories;
using AiWorkbench.Domain.Entities;
using AiWorkbench.Infrastructure.External;

namespace AiWorkbench.Infrastructure.Services
{
    public class ImageService(IAzureOpenAiClient ai, IBlobStorageService blob, IImageRepository repo) : IImageService
    {
        public async Task<ImageGenerationResponseDto> GenerateImageAsync(ImageGenerationRequestDto request, CancellationToken ct = default)
        {
            var bytes = await ai.GenerateImageAsync(request.Prompt, ct);

            var fileName = $"{DateTime.Now:yyyyMMdd_HHmmssfff}_{Random.Shared.Next(1000, 9999)}.png";
            var url = await blob.UploadAsync(fileName, bytes, "image/png", ct);

            var entity = new ImageGeneration
            {
                UserId = request.UserId,
                Prompt = request.Prompt,
                BlobUrl = url
            };

            await repo.AddAsync(entity, ct);

            return new ImageGenerationResponseDto { ImageUrl = url };
        }
        public async Task<IReadOnlyList<ImageGeneration>> ListByUserAsync(Guid userId, int skip = 0, int take = 50, CancellationToken ct = default)
        {
            return await repo.ListByUserAsync(userId, skip, take, ct);
        }
    }
}
using AiWorkbench.Application.DTOs.Images;
using AiWorkbench.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AiWorkbench.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/images")]
    public class ImageController(IImageService service) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Generate(ImageGenerationRequestDto dto, CancellationToken ct)
        {
            var result = await service.GenerateImageAsync(dto, ct);
            return Ok(result);
        }

        [HttpGet("history/{userId:guid}")]
        public async Task<IActionResult> GetHistory(Guid userId, int skip = 0, int take = 50, CancellationToken ct = default)
        {
            var items = await service.ListByUserAsync(userId, skip, take, ct);

            var result = items
                .OrderByDescending(i => i.CreatedAt)
                .Select(i => new
                {
                    from = "ai",
                    text = i.Prompt,
                    imageUrl = i.BlobUrl,
                    createdAt = i.CreatedAt
                })
                .ToList();

            return Ok(result);
        }
    }
}
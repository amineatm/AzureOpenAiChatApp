using AiWorkbench.Api.Extensions;
using AiWorkbench.API.DTOs.Documents;
using AiWorkbench.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AiWorkbench.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/documents")]
    public class DocumentController(IDocumentService service) : ControllerBase
    {
        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] DocumentUploadRequestDto dto, CancellationToken ct)
        {
            using var ms = new MemoryStream();
            await dto.File.CopyToAsync(ms, ct);
            byte[] fileBytes = ms.ToArray();
            var result = await service.UploadAsync(dto.UserId, dto.File.FileName, dto.File.ContentType, fileBytes, ct);

            return Ok(result);
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyDocument(CancellationToken ct)
        {
            var userId = User.GetUserId();
            var doc = await service.GetMyDocument(userId, ct);
            return Ok(doc);

        }

        [HttpDelete("me")]
        public async Task<IActionResult> DeleteMyDocument(CancellationToken ct)
        {
            var userId = User.GetUserId();
            await service.DeleteUserDocumentAsync(userId, ct);
            return NoContent();
        }
    }
}

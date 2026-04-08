using AiWorkbench.Application.DTOs.RAG;
using AiWorkbench.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AiWorkbench.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/rag")]
    public class RagController(IRagService service) : ControllerBase
    {
        [HttpPost("query")]
        public async Task<IActionResult> Query(RagQueryRequestDto dto, CancellationToken ct)
        {
            var result = await service.QueryAsync(dto, ct);

            return Ok(result);
        }

        [HttpGet("history/{userId}")]
        public async Task<IActionResult> GetHistory(Guid userId, CancellationToken ct)
        {
            var history = await service.GetHistoryAsync(userId, ct);
            return Ok(history);
        }
    }

}
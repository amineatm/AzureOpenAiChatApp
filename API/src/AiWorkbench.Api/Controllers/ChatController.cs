using AiWorkbench.Application.DTOs.Chat;
using AiWorkbench.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AiWorkbench.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/chat")]
    public class ChatController(IChatService service) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> SendMessage(ChatRequestDto dto, CancellationToken ct)
        {
            var result = await service.SendMessageAsync(dto, ct);
            return Ok(result);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetHistory(Guid userId, CancellationToken ct)
        {
            var history = await service.GetHistoryAsync(userId, ct);
            return Ok(history);
        }

    }
}
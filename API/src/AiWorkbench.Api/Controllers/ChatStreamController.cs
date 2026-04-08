using AiWorkbench.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AiWorkbench.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatStreamController(IChatStreamService chatStream) : ControllerBase
    {
        [HttpGet("stream")]
        public async Task Stream([FromQuery] string text)
        {
            Response.Headers.Append("Content-Type", "text/event-stream");

            await foreach (var chunk in chatStream.StreamAsync(text))
            {
                await Response.WriteAsync($"data: {chunk}\n\n");
                await Response.Body.FlushAsync();
            }
        }
    }
}

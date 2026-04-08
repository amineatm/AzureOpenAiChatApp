using AiWorkbench.Api.DTOs.Speech;
using AiWorkbench.Api.Extensions;
using AiWorkbench.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AiWorkbench.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/speech")]
    public class SpeechController(ISpeechService service) : ControllerBase
    {
        [HttpPost("stt")]
        public async Task<IActionResult> SpeechToText([FromForm] SpeechToTextRequestDto dto, CancellationToken ct)
        {
            using var ms = new MemoryStream();
            await dto.AudioFile.CopyToAsync(ms, ct);

            var appDto = new Application.DTOs.Speech.SpeechToTextRequestDto
            {
                UserId = User.GetUserId(),
                AudioBytes = ms.ToArray()
            };

            var result = await service.SpeechToTextAsync(appDto, ct);
            return Ok(result);
        }

        [HttpPost("tts")]
        public async Task<IActionResult> TextToSpeech(TextToSpeechRequestDto dto, CancellationToken ct)
        {
            var appDto = new Application.DTOs.Speech.TextToSpeechRequestDto
            {
                UserId = User.GetUserId(),
                Text = dto.Text
            };

            var result = await service.TextToSpeechAsync(appDto, ct);
            return File(result.AudioBytes, "audio/wav");
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetHistory(CancellationToken ct)
        {
            var userId = User.GetUserId();
            var sessions = await service.GetHistoryAsync(userId, ct);
            return Ok(sessions);
        }
    }
}
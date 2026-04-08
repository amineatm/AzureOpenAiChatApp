using AiWorkbench.Application.DTOs.Speech;
using AiWorkbench.Domain.Entities;

namespace AiWorkbench.Application.Interfaces
{
    public interface ISpeechService
    {
        Task<SpeechToTextResponseDto> SpeechToTextAsync(SpeechToTextRequestDto request, CancellationToken ct = default);
        Task<TextToSpeechResponseDto> TextToSpeechAsync(TextToSpeechRequestDto request, CancellationToken ct = default);
        Task<List<SpeechSession>> GetHistoryAsync(Guid userId, CancellationToken ct = default);
    }
}
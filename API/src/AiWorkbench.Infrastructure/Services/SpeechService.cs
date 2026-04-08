using AiWorkbench.Application.DTOs.Speech;
using AiWorkbench.Application.Interfaces;
using AiWorkbench.Application.Repositories;
using AiWorkbench.Domain.Entities;
using AiWorkbench.Infrastructure.External;

namespace AiWorkbench.Infrastructure.Services
{
    public class SpeechService(IAzureOpenAiClient azure, IBlobStorageService blob, ISpeechSessionRepository repo) : ISpeechService
    {
        public async Task<SpeechToTextResponseDto> SpeechToTextAsync(SpeechToTextRequestDto request, CancellationToken ct = default)
        {
            var audioFileName = $"speechtotext/{Guid.NewGuid()}.webm";
            var audioUrl = await blob.UploadAsync(audioFileName, request.AudioBytes, "audio/webm", ct);

            var text = await azure.SpeechToTextAsync(request.AudioBytes, ct);

            if (string.IsNullOrWhiteSpace(text) || text == ".")
            {
                return new SpeechToTextResponseDto { Text = string.Empty };
            }

            var session = new SpeechSession
            {
                UserId = request.UserId,
                SessionType = "transcription",
                SourceBlobUrl = audioUrl,
                ResultBlobUrl = null,
                TranscribedText = text,
                CompletedAt = DateTime.Now
            };

            await repo.AddAsync(session, ct);
            return new SpeechToTextResponseDto { Text = text };
        }

        public async Task<TextToSpeechResponseDto> TextToSpeechAsync(TextToSpeechRequestDto request, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(request.Text) || request.Text.Trim() == ".")
            {
                return new TextToSpeechResponseDto { AudioBytes = [] };
            }
            var audioBytes = await azure.TextToSpeechAsync(request.Text, ct);

            var fileName = $"texttospeech/{Guid.NewGuid()}.wav";
            var audioUrl = await blob.UploadAsync(fileName, audioBytes, "audio/wav", ct);
            var session = new SpeechSession
            {
                UserId = request.UserId,
                SessionType = "tts",
                SourceBlobUrl = null,
                GeneratedText = request.Text,
                ResultBlobUrl = audioUrl,
                CompletedAt = DateTime.Now
            };

            await repo.AddAsync(session, ct);

            return new TextToSpeechResponseDto { AudioBytes = audioBytes };
        }
        public async Task<List<SpeechSession>> GetHistoryAsync(Guid userId, CancellationToken ct = default)
        {
            return await repo.GetUserSessionsAsync(userId, ct);
        }
    }
}
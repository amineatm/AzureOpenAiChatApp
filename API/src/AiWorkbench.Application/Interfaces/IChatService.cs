using AiWorkbench.Application.DTOs.Chat;

namespace AiWorkbench.Application.Services.Interfaces
{
    public interface IChatService
    {
        Task<ChatResponseDto> SendMessageAsync(ChatRequestDto request, CancellationToken ct = default);
        Task<List<ChatMessageDto>> GetHistoryAsync(Guid userId, CancellationToken ct = default);

    }
}

using AiWorkbench.Application.DTOs.Chat;
using AiWorkbench.Application.Interfaces;
using AiWorkbench.Application.Repositories;
using AiWorkbench.Domain.Entities;
using AiWorkbench.Infrastructure.External;
using OpenAI.Chat;

namespace AiWorkbench.Infrastructure.Services
{
    public class ChatService(IAzureOpenAiClient ai, IGeneralChatRepository repo) : IChatService
    {
        public async Task<ChatResponseDto> SendMessageAsync(ChatRequestDto request, CancellationToken ct = default)
        {
            var history = await repo.GetByUserAsync(request.UserId, ct);

            var messages = new List<ChatMessage> { ChatMessage.CreateSystemMessage(ai.SystemPrompt) };

            foreach (var msg in history)
            {
                if (msg.Role == "user")
                    messages.Add(ChatMessage.CreateUserMessage(msg.Message));
                else
                    messages.Add(ChatMessage.CreateAssistantMessage(msg.Message));
            }

            messages.Add(ChatMessage.CreateUserMessage(request.Message));

            var userMessage = new GeneralChatMessage
            {
                UserId = request.UserId,
                Role = "user",
                Message = request.Message
            };
            await repo.AddAsync(userMessage, ct);

            var completion = await ai.ChatWithHistoryAsync(messages, ct);

            var assistantMessage = new GeneralChatMessage
            {
                UserId = request.UserId,
                Role = "assistant",
                Message = completion
            };
            await repo.AddAsync(assistantMessage, ct);

            return new ChatResponseDto { Response = completion };
        }


        public async Task<List<ChatMessageDto>> GetHistoryAsync(Guid userId, CancellationToken ct = default)
        {
            var messages = await repo.GetByUserAsync(userId, ct);

            return [.. messages.OrderBy(m => m.CreatedAt).Select(m => new ChatMessageDto
            {
                From = m.Role,
                Text = m.Message,
                CreatedAt = m.CreatedAt
            })];
        }
    }
}
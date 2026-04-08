using AiWorkbench.Application.Interfaces;
using AiWorkbench.Infrastructure.External;
using OpenAI.Chat;
using System.Runtime.CompilerServices;

namespace AiWorkbench.Infrastructure.Services;

public class ChatStreamService(IAzureOpenAiClient client) : IChatStreamService
{
    public async IAsyncEnumerable<string> StreamAsync(string text, [EnumeratorCancellation] CancellationToken ct = default)
    {
        var messages = new List<ChatMessage>
        {
            ChatMessage.CreateSystemMessage(client.SystemPrompt),
            ChatMessage.CreateUserMessage(text)
        };

        await foreach (var update in client.StreamChatAsync(messages))
        {
            if (update != null)
                yield return update;
        }
    }
}

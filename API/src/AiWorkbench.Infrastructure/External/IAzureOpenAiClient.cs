using OpenAI.Chat;

namespace AiWorkbench.Infrastructure.External
{
    public interface IAzureOpenAiClient
    {
        string SystemPrompt { get; }
        Task<string> ChatAsync(string userMessage, CancellationToken ct = default);
        Task<string> ChatWithHistoryAsync(List<ChatMessage> messages, CancellationToken ct = default);
        Task<string> RagChatAsync(string prompt, string context, CancellationToken ct = default);

        Task<float[]> GenerateEmbeddingAsync(string text, CancellationToken ct = default);

        Task<byte[]> GenerateImageAsync(string prompt, CancellationToken ct = default);

        Task<string> SpeechToTextAsync(byte[] audioBytes, CancellationToken ct = default);

        Task<byte[]> TextToSpeechAsync(string text, CancellationToken ct = default);
        IAsyncEnumerable<string> StreamChatAsync(List<ChatMessage> messages);
    }
}

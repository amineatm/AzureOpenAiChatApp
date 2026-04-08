using AiWorkbench.Application.Configuration;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Options;
using OpenAI.Audio;
using OpenAI.Chat;
using OpenAI.Embeddings;
using OpenAI.Images;

namespace AiWorkbench.Infrastructure.External
{
    public class AzureOpenAiClient : IAzureOpenAiClient
    {
        private readonly AzureOpenAIClient client;

        private readonly ChatClient chatClient;
        private readonly EmbeddingClient embeddingClient;
        private readonly ImageClient imageClient;

        private readonly AudioClient audioTranscriptionClient;
        private readonly AudioClient audioSpeechClient;

        private readonly string systemPrompt;
        public string SystemPrompt => systemPrompt;

        public AzureOpenAiClient(IOptions<ExternalServicesSettings> settings)
        {
            var s = settings.Value.AzureOpenAI;

            client = new AzureOpenAIClient(new Uri(s.Endpoint), new AzureKeyCredential(s.ApiKey));

            systemPrompt = s.ChatModeSystemMessage;

            chatClient = client.GetChatClient(s.ChatDeployment);
            embeddingClient = client.GetEmbeddingClient(s.EmbeddingDeployment);
            imageClient = client.GetImageClient(s.ImageDeployment);

            audioTranscriptionClient = client.GetAudioClient(s.AudioDeployment.AudioTranscriptionDeployment);
            audioSpeechClient = client.GetAudioClient(s.AudioDeployment.AudioSpeechDeployment);
        }

        public async Task<string> ChatAsync(string userMessage, CancellationToken ct = default)
        {
            ChatMessage[] messages =
            [
                ChatMessage.CreateSystemMessage(systemPrompt),
                ChatMessage.CreateUserMessage(userMessage)
            ];

            var completion = await chatClient.CompleteChatAsync(messages, cancellationToken: ct);
            return completion.Value.Content[0].Text;
        }

        public async Task<string> ChatWithHistoryAsync(List<ChatMessage> messages, CancellationToken ct = default)
        {
            var completion = await chatClient.CompleteChatAsync(messages, cancellationToken: ct);
            return completion.Value.Content[0].Text;
        }

        public async Task<string> RagChatAsync(string prompt, string context, CancellationToken ct = default)
        {
            var fullPrompt = $"Context:\n{context}\n\nUser:\n{prompt}";

            var completion = await chatClient.CompleteChatAsync(
                [ChatMessage.CreateUserMessage(fullPrompt)],
                cancellationToken: ct
            );

            return completion.Value.Content[0].Text;
        }

        public async Task<float[]> GenerateEmbeddingAsync(string text, CancellationToken ct = default)
        {
            var embedding = await embeddingClient.GenerateEmbeddingAsync(text, cancellationToken: ct);
            return embedding.Value.ToFloats().ToArray();
        }

        public async Task<byte[]> GenerateImageAsync(string prompt, CancellationToken ct = default)
        {
            var image = await imageClient.GenerateImageAsync(
                prompt,
                new ImageGenerationOptions
                {
                    Quality = GeneratedImageQuality.Standard,
                    Size = GeneratedImageSize.W1024xH1024,
                    ResponseFormat = GeneratedImageFormat.Bytes
                },
                ct
            );

            return image.Value.ImageBytes.ToArray();
        }

        public async Task<string> SpeechToTextAsync(byte[] audioBytes, CancellationToken ct = default)
        {
            await using var stream = new MemoryStream(audioBytes);
            if (audioBytes == null || audioBytes.Length < 3000)
            {
                return string.Empty;
            }

            var options = new AudioTranscriptionOptions
            {
                Language = "en",
                Temperature = 0.0f,
                ResponseFormat = AudioTranscriptionFormat.Simple,
                TimestampGranularities = AudioTimestampGranularities.Word
            };

            var result = await audioTranscriptionClient.TranscribeAudioAsync(stream, "audio.webm", options, ct);

            return result.Value.Text;
        }

        public async Task<byte[]> TextToSpeechAsync(string text, CancellationToken ct = default)
        {
            var options = new SpeechGenerationOptions
            {
                SpeedRatio = 1.0f
            };

            var result = await audioSpeechClient.GenerateSpeechAsync(
                text,
                GeneratedSpeechVoice.Alloy,
                options,
                ct
            );

            return result.Value.ToArray();
        }

        public async IAsyncEnumerable<string> StreamChatAsync(List<ChatMessage> messages)
        {
            await foreach (var update in chatClient.CompleteChatStreamingAsync(messages))
            {
                var delta = update.ContentUpdate;

                if (delta != null)
                {
                    foreach (var item in delta)
                    {
                        if (item.Text != null)
                        {
                            yield return item.Text;
                        }
                    }
                }
            }
        }
    }
}

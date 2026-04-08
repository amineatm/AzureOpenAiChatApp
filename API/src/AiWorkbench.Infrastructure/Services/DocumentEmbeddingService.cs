using AiWorkbench.Application.Interfaces;
using AiWorkbench.Application.Repositories;
using AiWorkbench.Domain.Entities;
using AiWorkbench.Infrastructure.External;
using System.Text.Json;

namespace AiWorkbench.Infrastructure.Services
{
    public class DocumentEmbeddingService(IFileTextExtractor extractor, ITextChunker chunker, IAzureOpenAiClient ai, IRagRepository rag) : IDocumentEmbeddingService
    {
        public async Task ProcessAsync(UploadedDocument doc, byte[] bytes, CancellationToken ct = default)
        {
            var text = extractor.ExtractText(doc.FileName, bytes);
            if (string.IsNullOrWhiteSpace(text))
                return;

            var chunks = chunker.Chunk(text, 200);

            var list = new List<DocumentEmbedding>();

            for (int i = 0; i < chunks.Count; i++)
            {
                var vector = await ai.GenerateEmbeddingAsync(chunks[i], ct);

                list.Add(new DocumentEmbedding
                {
                    DocumentId = doc.Id,
                    ChunkIndex = i,
                    EmbeddingJson = JsonSerializer.Serialize(vector),
                    TextSnippet = chunks[i]
                });
            }

            await rag.AddManyEmbeddingsAsync(list, ct);
        }
    }
}

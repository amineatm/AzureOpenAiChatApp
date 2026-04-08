using AiWorkbench.Application.DTOs.RAG;
using AiWorkbench.Application.Interfaces;
using AiWorkbench.Application.Repositories;
using AiWorkbench.Domain.Entities;
using AiWorkbench.Infrastructure.External;

namespace AiWorkbench.Infrastructure.Services
{
    public class RagService(IAzureOpenAiClient ai, IRagRepository ragRepo, IDocumentRepository docRepo) : IRagService
    {
        public async Task<RagQueryResponseDto> QueryAsync(RagQueryRequestDto request, CancellationToken ct = default)
        {
            var doc = await docRepo.GetByIdAsync(request.SourceDocumentId, ct);
            var fileName = doc?.FileName ?? "Document";

            await ragRepo.AddAsync(new RagChatMessage
            {
                UserId = request.UserId,
                Role = "user",
                Message = $"📄 {fileName}\n\n{request.Question}",
                SourceDocumentId = request.SourceDocumentId
            }, ct);

            var queryEmbedding = await ai.GenerateEmbeddingAsync(request.Question, ct);
            var chunks = await ragRepo.GetEmbeddingsByDocumentIdAsync(request.SourceDocumentId, 10, ct);
            var context = string.Join("\n\n", chunks.Select(c => c.TextSnippet));
            var answer = await ai.RagChatAsync(request.Question, context, ct);

            await ragRepo.AddAsync(new RagChatMessage
            {
                UserId = request.UserId,
                Role = "assistant",
                Message = answer,
                SourceDocumentId = request.SourceDocumentId
            }, ct);

            return new RagQueryResponseDto
            {
                Answer = answer,
                SourceDocumentId = request.SourceDocumentId,
                CreatedAt = DateTime.Now
            };
        }

        public Task<List<RagChatMessage>> GetHistoryAsync(Guid userId, CancellationToken ct)
        {
            return ragRepo.GetHistoryAsync(userId, ct);
        }
    }
}

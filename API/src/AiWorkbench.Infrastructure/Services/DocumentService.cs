using AiWorkbench.Application.DTOs.Documents;
using AiWorkbench.Application.Interfaces;
using AiWorkbench.Application.Repositories;
using AiWorkbench.Domain.Entities;
using AiWorkbench.Infrastructure.External;

namespace AiWorkbench.Infrastructure.Services
{
    public class DocumentService(IBlobStorageService blob, IDocumentRepository repo, IDocumentEmbeddingService embedding) : IDocumentService
    {
        public async Task<DocumentUploadResponseDto> UploadAsync(Guid userId, string fileName, string contentType, byte[] bytes, CancellationToken ct = default)
        {
            fileName  = $"Documents Embeddings/{fileName}";
            var url = await blob.UploadAsync(fileName, bytes, contentType, ct);

            var doc = new UploadedDocument
            {
                UserId = userId,
                FileName = fileName,
                BlobUrl = url,
                ContentType = contentType,
                SizeBytes = bytes.Length,
                UploadedAt = DateTime.Now
            };

            await repo.AddAsync(doc, ct);

            await embedding.ProcessAsync(doc, bytes, ct);

            return new DocumentUploadResponseDto
            {
                DocumentId = doc.Id,
                Url = url
            };
        }

        public async Task<UploadedDocument?> GetMyDocument(Guid userId, CancellationToken ct)
        {
            return await repo.GetLatestForUserAsync(userId, ct);
        }

        public async Task DeleteUserDocumentAsync(Guid userId, CancellationToken ct)
        {
            var doc = await GetMyDocument(userId, ct);
            if (doc is null) return;

            await blob.DeleteAsync(doc.FileName, ct);
            await repo.DeleteAsync(doc, ct);
        }

    }
}

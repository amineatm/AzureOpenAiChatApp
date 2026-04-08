using AiWorkbench.Domain.Entities;

namespace AiWorkbench.Application.Interfaces
{
    public interface IDocumentEmbeddingService
    {
        Task ProcessAsync(UploadedDocument doc, byte[] bytes, CancellationToken ct = default);
    }
}

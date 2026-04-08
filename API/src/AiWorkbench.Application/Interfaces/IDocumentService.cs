using AiWorkbench.Application.DTOs.Documents;
using AiWorkbench.Domain.Entities;
namespace AiWorkbench.Application.Interfaces
{
    public interface IDocumentService
    {
        Task<DocumentUploadResponseDto> UploadAsync(Guid userId, string fileName, string contentType, byte[] bytes, CancellationToken ct = default);
        Task<UploadedDocument?> GetMyDocument(Guid userId, CancellationToken ct);
        Task DeleteUserDocumentAsync(Guid userId, CancellationToken ct);

    }


}

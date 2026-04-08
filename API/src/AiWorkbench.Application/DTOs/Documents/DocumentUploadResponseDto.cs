namespace AiWorkbench.Application.DTOs.Documents
{
    public class DocumentUploadResponseDto
    {
        public Guid DocumentId { get; set; }
        public string Url { get; set; } = default!;
    }
}

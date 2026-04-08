namespace AiWorkbench.API.DTOs.Documents
{
    public class DocumentUploadRequestDto
    {
        public Guid UserId { get; set; }
        public IFormFile File { get; set; } = default!;
    }
}

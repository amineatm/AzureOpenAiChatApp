namespace AiWorkbench.Application.DTOs.RAG
{
    public class RagQueryRequestDto
    {
        public Guid UserId { get; set; }
        public string Question { get; set; } = null!;
        public Guid SourceDocumentId { get; set; }
    }

}

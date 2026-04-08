namespace AiWorkbench.Application.DTOs.RAG
{
    public class RagQueryResponseDto
    {
        public string Answer { get; set; } = null!;
        public Guid? SourceDocumentId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

}

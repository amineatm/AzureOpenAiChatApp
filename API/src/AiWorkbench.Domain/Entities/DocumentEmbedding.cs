namespace AiWorkbench.Domain.Entities;

public class DocumentEmbedding
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid DocumentId { get; set; }
    public int ChunkIndex { get; set; } 
    public string EmbeddingJson { get; set; } = null!; 
    public string? TextSnippet { get; set; } 
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public UploadedDocument? Document { get; set; }
}
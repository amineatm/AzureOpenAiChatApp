namespace AiWorkbench.Domain.Entities;

public class UploadedDocument
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string FileName { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public long SizeBytes { get; set; }
    public string BlobUrl { get; set; } = null!; 
    public DateTime UploadedAt { get; set; } = DateTime.Now;

    public User? User { get; set; }
    public ICollection<DocumentEmbedding> Embeddings { get; set; } = [];
}
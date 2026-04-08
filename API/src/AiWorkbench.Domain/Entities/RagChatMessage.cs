namespace AiWorkbench.Domain.Entities;

public class RagChatMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string Role { get; set; } = null!; 
    public string Message { get; set; } = null!;
    public Guid? SourceDocumentId { get; set; } 
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public User? User { get; set; }
    public UploadedDocument? SourceDocument { get; set; }
}
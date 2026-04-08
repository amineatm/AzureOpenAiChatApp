namespace AiWorkbench.Domain.Entities;

public class ImageGeneration
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string Prompt { get; set; } = null!;
    public string BlobUrl { get; set; } = null!; 
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public User? User { get; set; }
}
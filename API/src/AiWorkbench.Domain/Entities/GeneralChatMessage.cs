namespace AiWorkbench.Domain.Entities;

public class GeneralChatMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string Role { get; set; } = null!;
    public string Message { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public User? User { get; set; }
}
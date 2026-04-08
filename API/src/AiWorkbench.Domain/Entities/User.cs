namespace AiWorkbench.Domain.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; } = null!;
    public required string PasswordHash { get; set; } = null!;
    public string Role { get; set; } = "User"; 
    public required string DisplayName { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];

    public ICollection<GeneralChatMessage> GeneralChatMessages { get; set; } = [];
    public ICollection<UploadedDocument> UploadedDocuments { get; set; } = [];
}
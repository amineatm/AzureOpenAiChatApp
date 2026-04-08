using AiWorkbench.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Token { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? RevokedAt { get; set; }
    public string? ReplacedByToken { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public bool IsExpired => DateTime.Now >= ExpiresAt;
    public bool IsRevoked => RevokedAt != null;
    public bool IsActive => !IsExpired && !IsRevoked;
}
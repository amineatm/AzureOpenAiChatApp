namespace AiWorkbench.Application.DTOs.Auth
{
    public class SessionDto
    {
        public string RefreshToken { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsActive { get; set; }
    }
}

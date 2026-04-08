namespace AiWorkbench.Application.DTOs.Auth
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;
    }
}

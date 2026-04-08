namespace AiWorkbench.Application.DTOs.Chat
{
    public class ChatResponseDto
    {
        public string Response { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

}

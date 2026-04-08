namespace AiWorkbench.Application.DTOs.Chat
{
    public class ChatRequestDto
    {
        public Guid UserId { get; set; }
        public string Message { get; set; } = null!;

    }

}

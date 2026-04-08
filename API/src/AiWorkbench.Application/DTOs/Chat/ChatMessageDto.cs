namespace AiWorkbench.Application.DTOs.Chat
{
    public class ChatMessageDto
    {
        public string From { get; set; } = default!;
        public string Text { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
    }
}

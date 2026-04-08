namespace AiWorkbench.Application.DTOs.Speech
{
    public class TextToSpeechRequestDto
    {
        public Guid UserId { get; set; }
        public string Text { get; set; } = null!;
    }
}
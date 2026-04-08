namespace AiWorkbench.Application.DTOs.Speech
{
    public class SpeechToTextRequestDto
    {
        public Guid UserId { get; set; }
        public byte[] AudioBytes { get; set; } = null!;
    }
}
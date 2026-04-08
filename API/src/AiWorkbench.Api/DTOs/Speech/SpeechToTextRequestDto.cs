namespace AiWorkbench.Api.DTOs.Speech
{
    public class SpeechToTextRequestDto
    {
        public IFormFile AudioFile { get; set; } = null!;
    }
}
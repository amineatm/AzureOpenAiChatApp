namespace AiWorkbench.Application.DTOs.Images
{
    public class ImageGenerationRequestDto
    {
        public Guid UserId { get; set; }
        public string Prompt { get; set; } = null!;
    }
}

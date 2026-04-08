namespace AiWorkbench.Application.DTOs.Images
{
    public class ImageGenerationResponseDto
    {
        public string ImageUrl { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}

namespace AiWorkbench.Domain.Entities;

public class SpeechSession
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string SessionType { get; set; } = null!; 
    public string? SourceBlobUrl { get; set; }   
    public string? ResultBlobUrl { get; set; }  
    public string? TranscribedText { get; set; } 
    public string? GeneratedText { get; set; }  
    public DateTime StartedAt { get; set; } = DateTime.Now;
    public DateTime? CompletedAt { get; set; }
    public User? User { get; set; }
}

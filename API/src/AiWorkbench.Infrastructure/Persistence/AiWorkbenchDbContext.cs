using AiWorkbench.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AiWorkbench.Infrastructure.Persistence;

public class AiWorkbenchDbContext(DbContextOptions<AiWorkbenchDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
    public DbSet<GeneralChatMessage> GeneralChatMessages { get; set; } = null!;
    public DbSet<RagChatMessage> RagChatMessages { get; set; } = null!;
    public DbSet<UploadedDocument> UploadedDocuments { get; set; } = null!;
    public DbSet<DocumentEmbedding> DocumentEmbeddings { get; set; } = null!;
    public DbSet<ImageGeneration> ImageGenerations { get; set; } = null!;
    public DbSet<SpeechSession> SpeechSessions { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AiWorkbenchDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
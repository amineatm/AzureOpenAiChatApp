using AiWorkbench.Application.Interfaces;
using AiWorkbench.Infrastructure.Services;
using AiWorkbench.Application.Repositories;
using AiWorkbench.Infrastructure.Repositories;
using AiWorkbench.Infrastructure.External;
using Microsoft.Extensions.DependencyInjection;

namespace AiWorkbench.Infrastructure.Extensions
{
    public static class InfrastructureExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            //services.AddScoped<IChatService, ChatService>();
            //services.AddScoped<IImageService, ImageService>();
            //services.AddScoped<IRagService, RagService>();
            //services.AddScoped<ISpeechService, SpeechService>();
            //services.AddScoped<IChatStreamService, ChatStreamService>();
            //services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            //services.AddScoped<IDocumentService, DocumentService>();
            //services.AddScoped<IFileTextExtractor, FileTextExtractor>();
            //services.AddScoped<ITextChunker, TextChunker>();
            //services.AddScoped<IDocumentEmbeddingService, DocumentEmbeddingService>();
            //services.AddScoped<IUserService, UserService>();

            //services.AddScoped<IDocumentRepository, DocumentRepository>();
            //services.AddScoped<IEmbeddingRepository, EmbeddingStore>();
            //services.AddScoped<IGeneralChatRepository, GeneralChatRepository>();
            //services.AddScoped<IImageRepository, ImageRepository>();
            //services.AddScoped<IRagRepository, RagRepository>();
            //services.AddScoped<ISpeechSessionRepository, SpeechSessionRepository>();
            //services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            //services.AddScoped<IUserRepository, UserRepository>();

            //services.AddScoped<IAzureOpenAiClient, AzureOpenAiClient>();
            //services.AddScoped<IBlobStorageService, BlobStorageService>();

            services.Scan(scan => scan.FromAssemblyOf<ChatService>()
                                       .AddClasses()
                                       .AsMatchingInterface()
                                       .WithScopedLifetime()
                                       );

            return services;
        }
    }
}
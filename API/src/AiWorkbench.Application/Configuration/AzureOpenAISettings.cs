namespace AiWorkbench.Application.Configuration
{
    public class AzureOpenAISettings
    {
        public string ApiKey { get; set; } = default!;
        public string Endpoint { get; set; } = default!;

        public string ChatDeployment { get; set; } = default!;
        public string EmbeddingDeployment { get; set; } = default!;
        public string ImageDeployment { get; set; } = default!;
        public AudioDeployment AudioDeployment { get; set; } = default!;

        public string ChatModeSystemMessage { get; set; } = default!;
    }

}

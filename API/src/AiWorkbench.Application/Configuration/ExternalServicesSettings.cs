namespace AiWorkbench.Application.Configuration
{
    public class ExternalServicesSettings
    {
        public AzureOpenAISettings AzureOpenAI { get; set; } = default!;
        public BlobStorageSettings AzureBlobStorage { get; set; } = default!;
        public RagSettings RagService { get; set; } = default!;
    }
}

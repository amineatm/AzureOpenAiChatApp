using AiWorkbench.Application.Configuration;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.Extensions.Options;

namespace AiWorkbench.Infrastructure.External
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly BlobContainerClient container;
        private readonly BlobServiceClient serviceClient;
        private readonly BlobStorageSettings settings;

        public BlobStorageService(IOptions<BlobStorageSettings> options)
        {
            settings = options.Value;

            serviceClient = new BlobServiceClient(settings.ConnectionString);
            container = serviceClient.GetBlobContainerClient(settings.ContainerName);

            container.CreateIfNotExists(PublicAccessType.None);
        }

        public async Task<string> UploadAsync(string fileName, byte[] data, string contentType, CancellationToken ct = default)
        {
            var blobClient = container.GetBlobClient(fileName);

            using var stream = new MemoryStream(data);

            await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = contentType }, cancellationToken: ct);

            if (!blobClient.CanGenerateSasUri)
                throw new InvalidOperationException("BlobClient cannot generate SAS URI. Ensure you are using a connection string with account key.");

            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = blobClient.BlobContainerName,
                BlobName = blobClient.Name,
                Resource = "b",
                ExpiresOn = DateTime.UtcNow.AddDays(1)
            };

            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            return blobClient.GenerateSasUri(sasBuilder).ToString();
        }

        public async Task<byte[]> DownloadAsync(string fileName, CancellationToken ct = default)
        {
            var blobClient = container.GetBlobClient(fileName);
            var response = await blobClient.DownloadContentAsync(ct);
            return response.Value.Content.ToArray();
        }

        public async Task DeleteAsync(string fileName, CancellationToken ct = default)
        {
            var blob = container.GetBlobClient(fileName);
            await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots, cancellationToken: ct);
        }

    }
}

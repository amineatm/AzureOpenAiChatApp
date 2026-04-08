namespace AiWorkbench.Infrastructure.External
{
    public interface IBlobStorageService
    {
        Task<string> UploadAsync(string fileName, byte[] data, string contentType, CancellationToken ct = default);

        Task<byte[]> DownloadAsync(string fileName, CancellationToken ct = default);

        Task DeleteAsync(string fileName, CancellationToken ct = default);

    }
}

namespace Infrastructure.Blob.Interfaces
{
    public interface IFileService
    {
        Task<string?> DownloadAsync(string blobUrl);
        Task UploadAsync(MemoryStream fileStreamToUpload, CancellationToken cancellationToken);
    }
}

namespace Infrastructure.Blob
{
    public interface IFileService
    {
        Task<string?> DownloadAsync(string blobUrl);
        Task UploadAsync(MemoryStream fileStreamToUpload, CancellationToken cancellationToken);
    }
}

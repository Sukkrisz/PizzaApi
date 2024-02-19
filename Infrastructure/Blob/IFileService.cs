namespace Infrastructure.Blob
{
    public interface IFileService
    {
        Task<MemoryStream> DownloadAsync(string name);
        Task UploadAsync(MemoryStream fileStreamToUpload);
    }
}

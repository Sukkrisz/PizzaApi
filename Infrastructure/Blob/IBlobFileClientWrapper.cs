namespace Infrastructure.Blob
{
    public interface IBlobFileClientWrapper
    {
        public Task<string?> DownloadFileAsync(string fileName);
        Task UploadAsync(Stream content);
    }
}

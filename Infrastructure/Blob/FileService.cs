using Azure.Storage.Blobs;

namespace Infrastructure.Blob
{
    public class FileService : IFileService
    {
        private const string CONTAINER_NAME = "txts";
        private readonly BlobServiceClient _azureClient;

        public FileService(BlobServiceClient client)
        {
            _azureClient = client;
        }

        public async Task<MemoryStream> DownloadAsync(string name)
        {
            var containerClient = _azureClient.GetBlobContainerClient(CONTAINER_NAME);
            var fileClient = containerClient.GetBlobClient(name);

            var ms = new MemoryStream();
            await fileClient.DownloadToAsync(ms);

            return ms;
        }
    }
}

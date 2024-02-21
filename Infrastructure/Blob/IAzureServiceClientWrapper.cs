using Azure.Storage.Blobs;

namespace Infrastructure.Blob
{
    public interface IAzureServiceClientWrapper
    {
        public IBlobCotainerClientWrapper GetBlobContainerClient(string containerName);
    }
}

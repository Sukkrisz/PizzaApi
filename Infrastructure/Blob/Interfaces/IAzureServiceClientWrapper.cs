using Azure.Storage.Blobs;

namespace Infrastructure.Blob.Interfaces
{
    public interface IAzureServiceClientWrapper
    {
        public IBlobCotainerClientWrapper GetBlobContainerClient(string containerName);
    }
}

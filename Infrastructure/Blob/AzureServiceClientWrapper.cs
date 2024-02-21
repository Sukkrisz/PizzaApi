using Azure.Storage.Blobs;

namespace Infrastructure.Blob
{
    public class AzureServiceClientWrapper : IAzureServiceClientWrapper
    {
        private readonly BlobServiceClient _serviceClient;

        public AzureServiceClientWrapper(string connectionString)
        {
            _serviceClient = new BlobServiceClient(connectionString);
        }

        public IBlobCotainerClientWrapper GetBlobContainerClient(string containerName)
        {
            var concreteContainerClient = _serviceClient.GetBlobContainerClient(containerName);
            var containerClient = new BlobContainerClientWrapper(concreteContainerClient);
            return containerClient;
        }
    }
}

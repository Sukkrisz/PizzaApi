using Azure.Storage.Blobs;
using Infrastructure.Blob.Interfaces;

namespace Infrastructure.Blob.Wrappers
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

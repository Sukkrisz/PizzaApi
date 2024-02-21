using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Infrastructure.Blob
{
    public class BlobContainerClientWrapper : IBlobCotainerClientWrapper
    {
        private readonly BlobContainerClient _containerClient;

        public BlobContainerClientWrapper(BlobContainerClient containerClient)
        {
            _containerClient = containerClient;
        }

        public IBlobFileClientWrapper GetBlobFileClient(string fileName)
        {
            var concreteClient = _containerClient.GetBlobClient(fileName);
            var wrapper = new BlobFileClientWrapper(concreteClient);
            return wrapper;
        }

        public string[] GetBlobNamesInContainer(BlobTraits traits, BlobStates states, CancellationToken cancellationToken)
        {
            var getResult = _containerClient.GetBlobs(traits, states, null, cancellationToken);
            return getResult.Select(b => b.Name).ToArray();
        }
    }
}

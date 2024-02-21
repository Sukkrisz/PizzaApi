using Azure.Storage.Blobs.Models;

namespace Infrastructure.Blob
{
    public interface IBlobCotainerClientWrapper
    {
        public IBlobFileClientWrapper GetBlobFileClient(string fileName);
        public string[] GetBlobNamesInContainer(BlobTraits traits, BlobStates states, CancellationToken cancellationToken);
    }
}

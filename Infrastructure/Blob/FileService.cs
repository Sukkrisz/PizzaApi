using Azure.Storage.Blobs;
using Models.Infrastructure;
using System.ComponentModel;

namespace Infrastructure.Blob
{
    public class FileService : IFileService
    {
        private readonly BlobServiceClient _azureClient;

        public FileService(BlobServiceClient client)
        {
            _azureClient = client;
        }

        public async Task<MemoryStream> DownloadAsync(string blobUrl)
        {
            // The container name could have been burnt it. It would spare some perf, but thisway it's more reusable
            var containerName = Path.GetDirectoryName(new Uri(blobUrl).AbsolutePath);
            containerName = containerName.TrimStart('\\').TrimStart('/');
            var fileName = Path.GetFileName(blobUrl);

            var containerClient = _azureClient.GetBlobContainerClient(containerName);
            var fileClient = containerClient.GetBlobClient(fileName);

            var ms = new MemoryStream();
            await fileClient.DownloadToAsync(ms);

            return ms;
        }

        public async Task UploadAsync(MemoryStream memoryStream)
        {
            var containerClient = _azureClient.GetBlobContainerClient(FileServiceConstants.ToppingBlobContainer);

            var blobResults = containerClient.GetBlobs(
                                                        Azure.Storage.Blobs.Models.BlobTraits.None,
                                                        Azure.Storage.Blobs.Models.BlobStates.None,
                                                        null, CancellationToken.None);
            var nextFileName = GetNextFileName(blobResults.Select(br => br.Name).ToArray());

            BlobClient blobClient = containerClient.GetBlobClient(nextFileName);
            await blobClient.UploadAsync(memoryStream, true);
        }

        private string GetNextFileName(string[] previousFileNames)
        {
            int highestNumber;
            if (previousFileNames.Any())
            {
                // Remove the first 9 charactes (Toppings_) and then the last 4 (.txt)
                highestNumber = previousFileNames.Select(s => s.Substring(9)).Select(s => s.Remove(s.Length - 4))
                                             .Select(sn => int.Parse(sn))
                                             .Max();
            }
            else
            {
                highestNumber = 0;
            }

            return $"{FileServiceConstants.ToppingBlobFileName}{highestNumber + 1}.txt";

        }
    }
}

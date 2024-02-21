using ModelLibrary.Infrastructure;

namespace Infrastructure.Blob
{
    public class FileService : IFileService
    {
        private readonly IAzureServiceClientWrapper _azureClient;

        public FileService(IAzureServiceClientWrapper client)
        {
            _azureClient = client;
        }

        public async Task<string?> DownloadAsync(string blobUrl)
        {
            // The container name could have been burnt it. It would spare some perf, but thisway it's more reusable
            var containerName = Path.GetDirectoryName(new Uri(blobUrl).AbsolutePath);
            containerName = containerName.TrimStart('\\').TrimStart('/');
            var fileName = Path.GetFileName(blobUrl);

            var containerClient = _azureClient.GetBlobContainerClient(containerName);
            var fileClient = containerClient.GetBlobFileClient(fileName);

            var downloadedFileContent = await fileClient.DownloadFileAsync(blobUrl);
            return downloadedFileContent;
        }

        public async Task UploadAsync(MemoryStream memoryStream, CancellationToken cancellationToken)
        {
            var containerClient = _azureClient.GetBlobContainerClient(FileServiceConstants.ToppingBlobContainer);

            // Get all the non-deleted file infos from Azure
            var foundFileNames = containerClient.GetBlobNamesInContainer(
                                                        Azure.Storage.Blobs.Models.BlobTraits.None,
                                                        Azure.Storage.Blobs.Models.BlobStates.None,
                                                        cancellationToken);
            var nextFileName = GetNextFileName(foundFileNames);

            var blobClient = containerClient.GetBlobFileClient(nextFileName);
            await blobClient.UploadAsync(memoryStream);
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

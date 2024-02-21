using Azure.Storage.Blobs;

namespace Infrastructure.Blob
{
    public class BlobFileClientWrapper : IBlobFileClientWrapper
    {
        private BlobClient _fileClient;

        public BlobFileClientWrapper(BlobClient fileClient)
        {
            _fileClient = fileClient;
        }

        public async Task<string?> DownloadFileAsync(string fileName)
        {
            using MemoryStream ms = new();
            await _fileClient.DownloadToAsync(ms);
            if (ms is not null)
            {
                if (ms.Length > 0)
                {
                    return System.Text.Encoding.UTF8.GetString(ms.ToArray());
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                return null;
            }
        }

        public async Task UploadAsync(Stream content)
        {
            await _fileClient.UploadAsync(content, true);
        }
    }
}

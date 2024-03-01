using Azure;
using Infrastructure.Blob;
using Infrastructure.Blob.Interfaces;
using Moq;

namespace Tests.Unit
{
    public sealed class FileServiceTests
    {
        private const string VALID_FILE_PATH = "https://pizzaTest.com/files/Toppings_1.txt";
        private const string NOT_EXISTING_FILE_PATH = "https://pizzaTest.com/files/404.txt";
        private const string EMPTY_FILE_PATH = "https://pizzaTest.com/files/Empty.txt";

        private const string NON_EXISTING_CONTAINER = "https://pizzaTest.com/not-exisitng/Toppings_1.txt";
        private const string NON_EXISTING_FILE = "https://pizzaTest.com/files/404.txt";
        private const string NONT_VALID_PATH = "Not a valid path";



        private string _validFileContent { get; set; }
        private string _validContainerName { get; set; }
        private string _validFileName { get; set; }

        private bool disposedValue;

        public FileServiceTests()
        {
            var validFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Test_Toppings_1.txt");
            using (var fs = new FileStream(validFilePath, FileMode.Open, FileAccess.Read))
            using (var sr = new StreamReader(fs))
            {
                string _validFileContent = sr.ReadToEnd();
            }

            var containerName = Path.GetDirectoryName(new Uri(VALID_FILE_PATH).AbsolutePath);
            _validContainerName = containerName.TrimStart('\\').TrimStart('/');
            _validFileName = Path.GetFileName(VALID_FILE_PATH);
        }

        // Sadly the downdloaded file contents can't be passed through, since the _validFileContent is generated at runtime, and that
        // can't be passed as inline data
        [Theory]
        [InlineData(VALID_FILE_PATH)]
        [InlineData(EMPTY_FILE_PATH)]
        [InlineData(NOT_EXISTING_FILE_PATH)]
        public async Task DownloadAsync_CallsWrapperDownloadAsync(string fileName)
        {
            // Arrange
            var expected = GetDownloadedFileContent(fileName);

            var mockBlobFileService = new Mock<IBlobFileClientWrapper>();
            mockBlobFileService.Setup(fs => fs.DownloadFileAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(expected));

            var mockContainerService = new Mock<IBlobCotainerClientWrapper>();
            mockContainerService.Setup(cs => cs.GetBlobFileClient(It.IsAny<string>()))
                .Returns(mockBlobFileService.Object);

            var mockAzureService = new Mock<IAzureServiceClientWrapper>();
            mockAzureService.Setup(az => az.GetBlobContainerClient(It.IsAny<string>()))
                .Returns(mockContainerService.Object)
                .Verifiable();

            FileService pizzaFileService = new FileService(mockAzureService.Object);

            // Act
            var actual = await pizzaFileService.DownloadAsync(fileName);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(VALID_FILE_PATH)]
        [InlineData(NON_EXISTING_CONTAINER)]
        [InlineData(NON_EXISTING_FILE)]
        public async void DownloadAsync_NotFoundPaths(string inputBlobUrl)
        {
            // Arrange
            var mockBlobFileService = new Mock<IBlobFileClientWrapper>();
            mockBlobFileService.Setup(fs => fs.DownloadFileAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(_validFileContent));

            // In truth, the DownloadFileAsync of the bloblFileService is the one, that's going to throw the exception in real life,
            // but since the container name is the input parameter of the container service, we can only bind the setup + exception
            // to this service
            var mockContainerService = new Mock<IBlobCotainerClientWrapper>();
            mockContainerService.Setup(fs => fs.GetBlobFileClient(It.Is<string>(s => s != _validFileName)))
                .Throws(new Azure.RequestFailedException(404, "BlobNotFound"));

            mockContainerService.Setup(cs => cs.GetBlobFileClient(It.Is<string>(s => s == _validFileName)))
                .Returns(mockBlobFileService.Object);

            var mockAzureService = new Mock<IAzureServiceClientWrapper>();
            mockAzureService.Setup(cs => cs.GetBlobContainerClient(It.Is<string>(s => s != _validContainerName)))
                .Throws(new Azure.RequestFailedException(404, "ContainerNotFound"));

            mockAzureService.Setup(cs => cs.GetBlobContainerClient(It.Is<string>(s => s == _validContainerName)))
                .Returns(mockContainerService.Object);

            FileService pizzaFileService = new FileService(mockAzureService.Object);

            if (inputBlobUrl != VALID_FILE_PATH)
                await Assert.ThrowsAsync<Azure.RequestFailedException>(() => pizzaFileService.DownloadAsync(inputBlobUrl));
            else
                pizzaFileService.DownloadAsync(inputBlobUrl);
        }

        [Theory]
        [InlineData("")]
        [InlineData(NONT_VALID_PATH)]
        [InlineData(null)]
        public async Task DownloadAsync_BadInputUrl(string blobUrl)
        {
            // Arrange
            var expected = GetDownloadedFileContent(blobUrl);

            var mockBlobFileService = new Mock<IBlobFileClientWrapper>();
            mockBlobFileService.Setup(fs => fs.DownloadFileAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(expected));

            var mockContainerService = new Mock<IBlobCotainerClientWrapper>();
            mockContainerService.Setup(cs => cs.GetBlobFileClient(It.IsAny<string>()))
                .Returns(mockBlobFileService.Object);

            var mockAzureService = new Mock<IAzureServiceClientWrapper>();
            mockAzureService.Setup(az => az.GetBlobContainerClient(It.IsAny<string>()))
                .Returns(mockContainerService.Object)
                .Verifiable();

            FileService pizzaFileService = new FileService(mockAzureService.Object);

            // Assert
            if (blobUrl != null)
                await Assert.ThrowsAsync<System.UriFormatException>(() => pizzaFileService.DownloadAsync(blobUrl));
            else
                await Assert.ThrowsAsync<ArgumentNullException>("uriString", () => pizzaFileService.DownloadAsync(blobUrl));
        }

        private string? GetDownloadedFileContent(string fileName)
        {
            switch(fileName)
            {
                case VALID_FILE_PATH:
                    return _validFileContent;
                case EMPTY_FILE_PATH:
                    return string.Empty;
                case NOT_EXISTING_FILE_PATH:
                    return null;
                default:
                    return "Error in file name mapping.";

            }
        }
    }
}

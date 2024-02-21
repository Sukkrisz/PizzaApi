using Infrastructure.Blob;
using Moq;

namespace Tests.Unit
{
    public sealed class FileServiceTests
    {
        private const string VALID_FILE_PATH = "https://pizzaTest.com/files/Toppings_1.txt";
        private const string NOT_EXISTING_FILE_PATH = "https://pizzaTest.com/files/404.txt";
        private const string EMPTY_FILE_PATH = "https://pizzaTest.com/files/Empty.txt";

        private string _validFileContent { get; set; }
        private bool disposedValue;

        public FileServiceTests()
        {
            var validFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Test_Toppings_1.txt");
            using (var fs = new FileStream(validFilePath, FileMode.Open, FileAccess.Read))
            using (var sr = new StreamReader(fs))
            {
                string _validFileContent = sr.ReadToEnd();
            }
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

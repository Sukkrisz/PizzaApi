using Database.Models.Pizza;
using Database.Repositories.Interfaces;
using Infrastructure.Blob.Interfaces;
using Infrastructure.Mediator;
using MediatR;
using ModelLibrary.Infrastructure;
using PizzaAPI.Dtos.Pizza;
using PizzaAPI.Mappers;

namespace PizzaAPI.Commands
{
    public static class InsertToppingsFromCloudCommand
    {
        public struct Request : IRequest<WrapperResult<Response>>
        {
            public string BlobUrl { get; set; }
        }

        public struct Response
        {
            public ToppingDto[] Toppings { get; set; }
        }

        public class Handler : IRequestHandler<Request, WrapperResult<Response>>
        {
            private readonly IFileService _fileService;
            private readonly IToppingRepo _toppingRepo;

            public Handler(IFileService fileService, IToppingRepo toppingRepo)
            {
                _fileService = fileService;
                _toppingRepo = toppingRepo;
            }

            public async Task<WrapperResult<Response>> Handle(Request request, CancellationToken cancellationToken)
            {
                // Download the file containing the toppings from blob storage
                var downloadedFileContent = await _fileService.DownloadAsync(request.BlobUrl);

                if (downloadedFileContent is not null)
                {
                    if (downloadedFileContent != string.Empty)
                    {
                        // Convert the file content to model objects
                        var toppings = GetToppingsFromString(downloadedFileContent);

                        // Save the model objects to the db
                        var success = await _toppingRepo.BulkInsertAsync(toppings);
                        if (success)
                        {
                            return WrapperResult<Response>.Ok(new Response()
                            {
                                Toppings = toppings.Select(t => t.ToDto()).ToArray() 
                            });
                        }
                        else
                        {
                            return WrapperResult<Response>.Failed("Db save failed");
                        }
                    }
                    else
                    {
                        return WrapperResult<Response>.Failed("The downloaded file did not contain any toppings.");
                    }
                }
                else
                {
                    return WrapperResult<Response>.Failed("The file download from the server failed.");
                }
            }

            // Using this kind of string splitting will result in the fastest result
            // Specially compared to string.split(';')
            private List<ToppingModel> GetToppingsFromString(string fileContent)
            {
                var res = new List<ToppingModel>();

                int startIndex = 0;
                int lineEndIndex = fileContent.IndexOf(FileServiceConstants.ToppingFileLineDelimeter);
                var fileLength = fileContent.Length;

                while (lineEndIndex != -1)
                {
                    // Get a row (representing 1 topping model object)
                    var toppingString = fileContent.Substring(startIndex, lineEndIndex - startIndex);

                    // Convert the row to an object
                    var toppingModel = ConvertToTopping(toppingString);
                    res.Add(toppingModel);

                    // Indexing out of the file would result in an exception. This prevents it.
                    if (lineEndIndex + 3 > fileLength)
                    {
                        break;
                    }

                    // +3 because: 1 is the ; character, 2 is \r, 3 is \n
                    startIndex = lineEndIndex + 3;
                    lineEndIndex = fileContent.IndexOf(FileServiceConstants.ToppingFileLineDelimeter, lineEndIndex + 3);
                }

                return res;
            }

            private ToppingModel ConvertToTopping(string toppingString)
            {
                var commaIndex = toppingString.IndexOf(FileServiceConstants.ToppingsFileValueDelimeter);
                var name = toppingString.Substring(0, commaIndex);
                var price = short.Parse(toppingString.Substring(commaIndex + 1));

                return new ToppingModel(name, price);
            }
        }
    }
}

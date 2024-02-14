﻿using Data.Db.Models.Pizza;
using Data.Db.Repositories.Interfaces;
using Infrastructure.Blob;
using Infrastructure.Mediator;
using MediatR;
using PizzaAPI.Dtos.Pizza;
using PizzaAPI.Mappers;

namespace PizzaAPI.Commands
{
    public static class InsertToppingsFromCloudCommand
    {
        public struct Request : IRequest<WrapperResult<Response>>
        {
        }

        public struct Response
        {
            public ToppingDto[] Toppings { get; set; }
        }

        public class Handler : IRequestHandler<Request, WrapperResult<Response>>
        {
            private const string TOPPINGS_FILE_NAME = "Toppings.txt";
            private const char LINE_DELIMETER = ';';
            private const char VALUES_DELIMETER = ',';

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
                var downloadedFile = await _fileService.DownloadAsync(TOPPINGS_FILE_NAME);

                if (downloadedFile is not null)
                {
                    if (downloadedFile.Length > 0)
                    {
                        // Convert the file content to model objects
                        var fileContent = System.Text.Encoding.UTF8.GetString(downloadedFile.ToArray());
                        var toppings = GetToppingsFromString(fileContent);

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
                int lineEndIndex = fileContent.IndexOf(LINE_DELIMETER);
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
                    lineEndIndex = fileContent.IndexOf(LINE_DELIMETER, lineEndIndex + 3);
                }

                return res;
            }

            private ToppingModel ConvertToTopping(string toppingString)
            {
                var commaIndex = toppingString.IndexOf(VALUES_DELIMETER);
                var name = toppingString.Substring(0, commaIndex);
                var price = ushort.Parse(toppingString.Substring(commaIndex + 1));

                return new ToppingModel(name, price);
            }
        }
    }
}
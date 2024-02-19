using Azure.Messaging.EventGrid.SystemEvents;
using Azure.Messaging.EventGrid;
using Microsoft.AspNetCore.Mvc;
using PizzaAPI.Commands;
using System.Text.Json;
using Core.Data.Repositories;
using Data.Db.Repositories.Interfaces;
using Azure;
using Infrastructure.Blob;

namespace PizzaAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ToppingController : BaseController
    {
        private readonly ILogger _logger;
        private readonly IToppingRepo _tr;
        private readonly IFileService _fs;

        public ToppingController(ILogger<OrderRepo> logger, IToppingRepo p, IFileService fs)
        {
            _logger = logger;
            _tr = p;
            _fs = fs;
        }

        // POST: ToppingController/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        [HttpPost("BulkImportCloud")]
        public async Task<ActionResult<InsertToppingsFromCloudCommand.Response>> BulkImportFromCloudFile()
        {
            var request = new InsertToppingsFromCloudCommand.Request();
            var res = await this.Mediator.Send(request);

            return this.FromWrapperResult(res);
        }

        // POST: ToppingController/Edit/5
        [HttpPut]
        public async Task<ActionResult> Edit(int id, IFormCollection collection)
        {
            try
            {
                return Ok();
            }
            catch
            {
                return View();
            }
        }

        // GET: ToppingController/Delete/5
        [HttpDelete]
        public ActionResult Delete(int id)
        {
            return View();
        }

        [HttpPost("HandleEvt")]
        public async Task<IActionResult> HandleEvt([FromBody] EventGridEvent[] events)
        {
            foreach (var evt in events)
            {
                // Check if it is a system event
                if (evt.TryGetSystemEventData(out object eventData))
                {
                    // Check if it is a subscription validation event
                    if (eventData is SubscriptionValidationEventData subscriptionValidationEventData)
                    {
                        // Return the validation code
                        var responseData = new
                        {
                            ValidationResponse = subscriptionValidationEventData.ValidationCode
                        };
                        return new OkObjectResult(responseData);
                    }
                    else if (evt.EventType == SystemEventNames.StorageBlobCreated)
                    {
                        var evtData = (StorageBlobCreatedEventData)eventData;
                        var request = new InsertToppingsFromCloudCommand.Request() { BlobUrl = evtData.Url };
                        await this.Mediator.Send(request);

                        return Ok();
                    }
                    
                }
            }

            // Return BadRequest for unrecognized events
            return BadRequest();
        }

        [HttpPost("Upload")]
        public async Task<ActionResult> Upload(IFormFile file)
        {
            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms); // Copy content from IFormFile to MemoryStream
                ms.Seek(0, SeekOrigin.Begin); // Seek to the beginning of the MemoryStream
                await _fs.UploadAsync(ms);
            }
            return Ok();
        }

    }
}

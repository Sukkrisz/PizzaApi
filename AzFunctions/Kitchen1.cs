using Data.Db.Repositories.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask.Entities;
using Microsoft.Extensions.Logging;

namespace AzFunctions
{
    public class Kitchen1 : TaskEntity<List<int>>
    {
        readonly ILogger logger;
        readonly IOrderRepo _orderRepo;

        public Kitchen1(ILogger<Counter> logger, IOrderRepo orderRepo)
        {
            this.logger = logger;
            _orderRepo = orderRepo;
        }

        public List<int> GetOrdersToPhoneNumber(string phoneNumber)
        {
            logger.LogInformation("In the kitchen");
            logger.LogInformation("Is null? {x}", _orderRepo is null);

            var orderIds = (_orderRepo.GetOrdersToPhoneNumber(phoneNumber)).GetAwaiter().GetResult();
            return orderIds.ToList();
        }

        [Function(nameof(Kitchen1))]
        public Task RunEntityAsync([EntityTrigger] TaskEntityDispatcher dispatcher)
        {
            return dispatcher.DispatchAsync(this);
        }
    }
}

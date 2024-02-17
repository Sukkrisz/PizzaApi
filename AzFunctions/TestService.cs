using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask.Entities;
using Microsoft.Extensions.Logging;
using Models.Shared.Func;

namespace AzFunctions
{
    internal class TestService : TaskEntity<RestaurantEntity>
    {
        readonly ILogger logger;

        public TestService(ILogger<Counter> logger)
        {
            this.logger = logger;
            this.State = new RestaurantEntity();
        }

        public RestaurantEntity Get()
        {
            //logger.LogInformation($"Get called");
            return this.State;
        }

        [Function(nameof(TestService))]
        public Task RunEntityAsync([EntityTrigger] TaskEntityDispatcher dispatcher)
        {
            return dispatcher.DispatchAsync(this);
        }
    }
}

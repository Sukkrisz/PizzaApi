using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask.Entities;
using Microsoft.Extensions.Logging;

namespace AzFunctions
{
    internal class AzStringBuilder : TaskEntity<List<string>>
    {
        readonly ILogger logger;

        public AzStringBuilder(ILogger<Counter> logger)
        {
            this.logger = logger;
            this.State = new List<string>();
        }

        public void Add(string s)
        {
            //logger.LogInformation($"String added: {s}");
            this.State.Add(s);
        }

        public void Reset()
        {
            this.State.Clear();
        }

        public List<string> Get()
        {
            //logger.LogInformation($"Get called");
            return this.State;
        }

        [Function(nameof(AzStringBuilder))]
        public Task RunEntityAsync([EntityTrigger] TaskEntityDispatcher dispatcher)
        {
            return dispatcher.DispatchAsync(this);
        }
    }
}

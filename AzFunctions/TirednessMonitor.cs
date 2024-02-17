using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask.Entities;
using Microsoft.Extensions.Logging;

namespace AzFunctions
{
    internal class TirednessMonitor : TaskEntity<double>
    {
        public static readonly string SERVICEKEY = "tirednessService_";
        private const int TIREDNESS_REDUCED_BY_BREAK = 30;
        private const int TIREDNESS_WHEN_BREAK = 100;

        readonly ILogger _logger;
        private EntityInstanceId _connectedKitchenInstanceId;

        private EntityInstanceId KitchenInstanceId
        {
            get
            {
                if(_connectedKitchenInstanceId.Name != default(string))
                {
                    return _connectedKitchenInstanceId;
                }
                else
                {
                    var delimeterIndex = this.Context.Id.Key.IndexOf('_');
                    var city = this.Context.Id.Key.Substring(delimeterIndex + 1);

                    _connectedKitchenInstanceId = new EntityInstanceId(nameof(Restaurant), Restaurant.SERVICEKEY + city);
                    return _connectedKitchenInstanceId;
                }
            }
        }

        public TirednessMonitor(ILogger<TirednessMonitor> logger)
        {
            this._logger = logger;
            logger.LogInformation("ctx status const: " + (this.Context is null).ToString());
        }

        public void Raise(double d)
        {
            this.State += d;
        }

        public double GetTeamTiredness()
        {
            this.Context.SignalEntity(new EntityInstanceId(nameof(Restaurant), Restaurant.SERVICEKEY + "Pecs"), nameof(Restaurant.Ping));
            return this.State;
        }

        public void IncreaseTiredness()
        {
            this.State += 1;

            if (State > TIREDNESS_WHEN_BREAK)
            {
                this.Context.SignalEntity(new EntityInstanceId(nameof(Restaurant), Restaurant.SERVICEKEY + "Pecs"), nameof(Restaurant.SendToBreak));
            }
        }

        public void ReduceTirednessByBreak()
        {
            _logger.LogWarning("Reducing tiredness");
            if (State - TIREDNESS_REDUCED_BY_BREAK > 0)
            {
                State -= TIREDNESS_REDUCED_BY_BREAK;
            }
            else
            {
                State = 0;
            }
        }

        [Function(nameof(TirednessMonitor))]
        public static Task Run([EntityTrigger] TaskEntityDispatcher dispatcher)
        => dispatcher.DispatchAsync<TirednessMonitor>();
    }
}

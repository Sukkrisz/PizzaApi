using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask.Entities;
using Microsoft.Extensions.Logging;
using Models.Shared;
using Models.Shared.Func;

namespace AzFunctions
{
    //Object?
    internal class Restaurant : TaskEntity<RestaurantEntity>
    {
        public static readonly string SERVICEKEY = "kitchenManager_";

        private readonly ILogger _logger;

        public Restaurant(ILogger<Restaurant> logger)
        {
            _logger = logger;
            this.State = new RestaurantEntity();
        }

        public void InitRestaurant(Cities city)
        {
            _logger.LogWarning("Init " + city.ToInternalString());
            if (city == Cities.Budapest)
            {
                this.State.WorkersCount = 4;
                this.State.OpeningHour = 9;
                this.State.ClosingHour = 23;
            }
            else if (city == Cities.Pécs)
            {
                this.State.WorkersCount = 2;
                this.State.OpeningHour = 11;
                this.State.ClosingHour = 21;
            }
        }

        public void Ping()
        {
            _logger.LogWarning("Pong!");
        }

        public uint GetWorkersCount()
        {
            return this.State.WorkersCount;
        }

        public void MakePizza(double timeToBake)
        {
            var entId = GetTirednessMonitoringInstanceId(Context.Id.Key);
            var timeToWait = (int)Math.Floor(timeToBake / this.State.WorkersCount);
            _logger.LogWarning("Baking " + timeToBake);
            this.Context.SignalEntity(entId, nameof(TirednessMonitor.IncreaseTiredness));
            Thread.Sleep(timeToWait);
        }

        public void SendToBreak()
        {
            if (this.State.BreakStart is null)
            {
                _logger.LogWarning("Sent to break ");
                this.State.BreakStart = DateTime.UtcNow;
                this.State.WorkersCount -= 1;
                this.Context.SignalEntity(GetTirednessMonitoringInstanceId(Context.Id.Key), nameof(TirednessMonitor.ReduceTirednessByBreak));

                Thread.Sleep(15 * 1000);

                this.Context.SignalEntity(Context.Id, nameof(EndBreak));
            }
        }

        public void EndBreak()
        {
            _logger.LogWarning("Break ended");
            if (this.State.WorkersCount < 3)
            {
                this.State.WorkersCount += 1;
            }

            this.State.BreakStart = null;
        }

        public bool IsRestaurantOpen(DateTime timeToCheck)
        {
            return IsRestaurantOpen(this.State.OpeningHour, this.State.ClosingHour, timeToCheck);
        }

        private static bool IsRestaurantOpen(uint openingHour, uint closingHour, DateTime timeToCheck)
        {
            var currentHour = timeToCheck.Hour + PizzaConstants.UtcDiff;
            return currentHour >= openingHour && currentHour <= closingHour;
        }

        private static EntityInstanceId GetTirednessMonitoringInstanceId(string ownKey)
        {
            var delimeterIndex = ownKey.IndexOf('_');
            var city = ownKey.Substring(delimeterIndex + 1);
            return new EntityInstanceId(nameof(TirednessMonitor), TirednessMonitor.SERVICEKEY + city);
        }

        [Function(nameof(Restaurant))]
        public static Task Run([EntityTrigger] TaskEntityDispatcher dispatcher)
        => dispatcher.DispatchAsync<Restaurant>();
    }
}

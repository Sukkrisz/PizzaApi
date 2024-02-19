using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask.Entities;
using Microsoft.Extensions.Logging;
using ModeLibrary.Shared.Func;
using ModelLibrary.Shared;
using ModelLibrary.Shared.Func;

namespace AzFunctions.DurableEntities
{
    // Using Object? as the type param for the TaskEntity would in theory persist all properties of the Restaurant class
    // In my experience it's not foolproof. Seen runtime errors, wouldn't recommend it.
    internal class Restaurant : TaskEntity<RestaurantEntity>
    {
        private readonly ILogger _logger;

        public Restaurant(ILogger<Restaurant> logger)
        {
            _logger = logger;
            State = new RestaurantEntity();
        }

        public void InitRestaurant(Cities city)
        {
            // Could be written in a generic way, where we store values connected to different cities
            // Then we wouldn't have to "burn in" any data
            if (city == Cities.Budapest)
            {
                State.WorkersCount = 4;
                State.OpeningHour = 9;
                State.ClosingHour = 23;
            }
            else if (city == Cities.Pécs)
            {
                State.WorkersCount = 2;
                State.OpeningHour = 11;
                State.ClosingHour = 21;
            }
        }

        public uint GetWorkersCount()
        {
            return State.WorkersCount;
        }

        public void MakePizza(double timeToBake)
        {
            // Increase the tiredness by 1. Wanted to send the timeToBake to the other durable function, but sending values
            // through doesn NOT work. The value (whatever type, even a simple int) will not be sent through and the receiving side
            // will miss the incoming parameter and lead to a runtime exception
            var entId = GetTirednessMonitoringInstanceId(Context.Id.Key);
            Context.SignalEntity(entId, nameof(TirednessMonitor.IncreaseTiredness));

            // Simulate the time workers are making the pizza.
            var timeToWait = (int)Math.Floor(timeToBake / State.WorkersCount);
            Thread.Sleep(timeToWait);
        }

        // If the tiredness reached lvl 100, 1 worker is sent to a break.
        // During this time there's 1 less employee working on orders, so it takes more time to make the incoming pizzas
        public void SendToBreak()
        {
            if (State.BreakStart is null)
            {
                State.BreakStart = DateTime.UtcNow;
                State.WorkersCount -= 1;

                // Reduce the tiredness by 30
                Context.SignalEntity(GetTirednessMonitoringInstanceId(Context.Id.Key), nameof(TirednessMonitor.ReduceTirednessByBreak));

                Thread.Sleep(15 * 1000);

                // An example on how to call one of it's own methods of an entity.
                Context.SignalEntity(Context.Id, nameof(EndBreak));
            }
        }

        public void EndBreak()
        {
            if (State.WorkersCount < 3)
            {
                State.WorkersCount += 1;
            }

            State.BreakStart = null;
        }

        public bool IsRestaurantOpen(DateTime timeToCheck)
        {
            return IsRestaurantOpenInternal(State.OpeningHour, State.ClosingHour, timeToCheck);
        }

        private static bool IsRestaurantOpenInternal(uint openingHour, uint closingHour, DateTime timeToCheck)
        {
            var currentHour = timeToCheck.Hour + PizzaConstants.UtcDiff;
            return currentHour >= openingHour && currentHour <= closingHour;
        }

        private static EntityInstanceId GetTirednessMonitoringInstanceId(string ownKey)
        {
            var delimeterIndex = ownKey.IndexOf('_');
            var city = ownKey.Substring(delimeterIndex + 1);
            return new EntityInstanceId(nameof(TirednessMonitor), AzFunctionConstants.TirednessServiceKey + city);
        }

        // There are conflicting ways on how to call "Run" in the MS documentations.
        // One shows it being done with RunAsync and dispatching with the entity's instance itself,
        // in an other one, it is explicity said, NOT to use a method called RunAsync, since it will lead to runtime errors.
        // As I've seen both works fine.
        //Here are the docs:
        // https://learn.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-dotnet-entities?pivots=isolated
        // https://learn.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-entities?tabs=class-based%2Cisolated-process%2Cpython-v2&pivots=csharp
        [Function(nameof(Restaurant))]
        public static Task Run([EntityTrigger] TaskEntityDispatcher dispatcher)
        => dispatcher.DispatchAsync<Restaurant>();
    }
}

using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask.Entities;
using Microsoft.Extensions.Logging;
using ModeLibrary.Shared.Func;

namespace AzFunctions.DurableEntities
{
    internal class TirednessMonitor : TaskEntity<double>
    {

        private const int TIREDNESS_REDUCED_BY_BREAK = 30;
        private const int TIREDNESS_WHEN_BREAK = 100;

        readonly ILogger _logger;
        private EntityInstanceId _connectedRestaurantInstanceId;

        // Seen runtime erros occuring, when I used this Getter way method in the Restaurant class.
        // Wouldn't recommend it, even though it's more usable.
        // Do use a private method, as I did in that class. It's safer.
        private EntityInstanceId RestaurantInstanceId
        {
            get
            {
                if (_connectedRestaurantInstanceId.Name != default)
                {
                    return _connectedRestaurantInstanceId;
                }
                else
                {
                    var delimeterIndex = Context.Id.Key.IndexOf(AzFunctionConstants.ServiceKeyDelimeter);
                    var city = Context.Id.Key.Substring(delimeterIndex + 1);

                    // Thisway we can call the restaurant service which belongs to the same city, the current tiredness entity belongs to
                    _connectedRestaurantInstanceId = new EntityInstanceId(nameof(Restaurant), AzFunctionConstants.RestaurantServiceKey + city);
                    return _connectedRestaurantInstanceId;
                }
            }
        }

        public TirednessMonitor(ILogger<TirednessMonitor> logger)
        {
            _logger = logger;
        }

        public double GetTeamTiredness()
        {
            return State;
        }

        // Called from the restaurant service. The processing of each pizza raises the team's tiredness by 1.
        public void IncreaseTiredness()
        {
            State += 1;

            if (State > TIREDNESS_WHEN_BREAK)
            {
                Context.SignalEntity(RestaurantInstanceId, nameof(Restaurant.SendToBreak));
            }
        }

        // Called from the restaurant, whenever an employee goes to break
        public void ReduceTirednessByBreak()
        {
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

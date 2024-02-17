using System.Diagnostics;
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Data.Db.Repositories.Interfaces;
using Infrastructure.Settings;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.DurableTask.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models.Shared;
using Models.Shared.Func;
using Shared.Dto;

namespace AzFunctions
{
    public class EntityFunction
    {
        private readonly ILogger<EntityFunction> _logger;
        private readonly IOptions<ConnectionStringSettings> _settings;
        private readonly IOrderRepo _orderRepo;

        public EntityFunction(ILogger<EntityFunction> logger, IOptions<ConnectionStringSettings> settings, IOrderRepo orderRepo)
        {
            _logger = logger;
            _settings = settings;
            _orderRepo = orderRepo;
        }

        [Function("EntityFunct_Starter")]
        public async Task Run(
                [ServiceBusTrigger(
                    topicName: "%OrdersTopic%",
                    subscriptionName: "%KitchenSubscription_Pecs%",
                    Connection = "ConnectionString",
                    IsSessionsEnabled = true)]
                ServiceBusReceivedMessage message,
                [DurableClient] DurableTaskClient client)
        {
            _logger.LogInformation("An order for the Bp restaurant!");

            var msgBody = JsonSerializer.Deserialize<OrderDto>(message.Body);
            var order = new Order(msgBody.PhoneNumber, msgBody.OrderDate, msgBody.Address.City);

            // InstanceId can be stored in the db for example connecting it with the order, and the whole running task can be shut down.
            // This would of course be usefull in case of cancellations or emergency shutdowns.
            string instanceId = await client.ScheduleNewOrchestrationInstanceAsync("Entity_Orchestrator", order);
        }

        [Function("Entity_Orchestrator")]
        public static async Task RunOrchestrator([OrchestrationTrigger] TaskOrchestrationContext context)
        {
            ILogger logger = context.CreateReplaySafeLogger("Entity_Orchestrator");
            var inputMsg = context.GetInput<Order>();

            try
            {
                var restaurantEntityId = new EntityInstanceId(nameof(Restaurant), Restaurant.SERVICEKEY + inputMsg.City.ToInternalString());
                
                var workersCount = await context.Entities.CallEntityAsync<uint>(restaurantEntityId, nameof(Restaurant.GetWorkersCount));
                //if (workersCount == 0)
                {
                    await context.Entities.SignalEntityAsync(restaurantEntityId, nameof(Restaurant.InitRestaurant), inputMsg.City);
                }

                var isRestauranOpen = await context.Entities.CallEntityAsync<bool>(
                                                                    restaurantEntityId,
                                                                    nameof(Restaurant.IsRestaurantOpen),
                                                                    inputMsg.OrderDate);
                if (isRestauranOpen)
                {
                    //GetMakeTimes
                    var pizzasWithBakeTime = await context.CallActivityAsync<IEnumerable<PizzaWithMakeTime>>(nameof(GetMakeTimesAsync), inputMsg);
                    var bakeTasks = new Task[pizzasWithBakeTime.Count()];

                    var tirednessEntityId = new EntityInstanceId(nameof(TirednessMonitor), TirednessMonitor.SERVICEKEY + "Pecs");
                    var tiredness = await context.Entities.CallEntityAsync<double>(tirednessEntityId, nameof(TirednessMonitor.GetTeamTiredness));
                    logger.LogWarning("Tiredness before: " + tiredness);

                    var stopwatch = new Stopwatch();
                    stopwatch.Start();
                    int i = 0;
                    foreach(var pizzaToBake in pizzasWithBakeTime)
                    {
                        bakeTasks[i] = context.Entities.SignalEntityAsync(restaurantEntityId, nameof(Restaurant.MakePizza), pizzaToBake.TimeToMake);
                        i += 1;
                    }

                    Task.WaitAll(bakeTasks);

                    tiredness = await context.Entities.CallEntityAsync<double>(tirednessEntityId, nameof(TirednessMonitor.GetTeamTiredness));
                    logger.LogWarning("Tiredness after: " + tiredness);
                }

                /*
                var tirednessEntityId = new EntityInstanceId(nameof(TirednessMonitor), TirednessMonitor.SERVICEKEY+"Pecs");
                await context.Entities.SignalEntityAsync(tirednessEntityId, nameof(TirednessMonitor.Raise), 3);
                var tiredness = await context.Entities.CallEntityAsync<double>(tirednessEntityId, nameof(TirednessMonitor.GetTeamTiredness));
                logger.LogWarning("Tiredness: " + tiredness);

                var kitchenEntityId = new EntityInstanceId(nameof(Restaurant), Restaurant.SERVICEKEY + "Pecs");
                await context.Entities.SignalEntityAsync(restaurantEntityId, nameof(Restaurant.Sell), 5);
                var psold = await context.Entities.CallEntityAsync<RestaurantEntity>(restaurantEntityId, nameof(Restaurant.GetSoldPizzasCount));
                logger.LogWarning("Sold: " + psold.PizzasSoldThisDay);
                await context.Entities.SignalEntityAsync(restaurantEntityId, nameof(Restaurant.Sell), 3);
                psold = await context.Entities.CallEntityAsync<RestaurantEntity>(restaurantEntityId, nameof(Restaurant.GetSoldPizzasCount));
                logger.LogWarning("Sold: " + psold.PizzasSoldThisDay);


                await context.Entities.SignalEntityAsync(tirednessEntityId, nameof(TirednessMonitor.IncreaseTiredness), 3);
                */
                /*var orders = await context.Entities.CallEntityAsync<List<int>>(entityId, "GetOrdersToPhoneNumber", msg.PhoneNumber);

                foreach (var id in orders)
                {
                    logger.LogInformation("OrderNum found {id}", id);
                }

                outputs.Add(await context.CallActivityAsync<string>(nameof(CallMeBaby), "Nancy"));
                outputs.Add(await context.CallActivityAsync<string>(nameof(CallMeBaby), "Julia"));

                logger.LogInformation(outputs[0]);
                logger.LogInformation(outputs[1]);*/
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }
        }

        [Function(nameof(GetMakeTimesAsync))]
        public IEnumerable<PizzaWithMakeTime> GetMakeTimesAsync([ActivityTrigger] Order input) 
        {
            try
            {
                var res = _orderRepo.GetMakeTimes(input.PhoneNumber, input.OrderDate).GetAwaiter().GetResult();
                _logger.LogWarning("Is null: " + (res == null).ToString());
                _logger.LogWarning("Count: " + res.Count());

                foreach(var p in res)
                {
                    _logger.LogWarning("Id: " + p.PizzaId + " Size:" + p.Size + " Time: " + p.TimeToMake);
                }
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            return null;
        }
    }
}

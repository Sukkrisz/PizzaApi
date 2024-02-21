using System.Text.Json;
using AzFunctions.Dtos;
using AzFunctions.DurableEntities;
using Azure.Messaging.ServiceBus;
using Database.Repositories.Interfaces;
using Infrastructure.Settings;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.DurableTask.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModeLibrary.Shared.Func;
using ModelLibrary.Shared;
using ModelLibrary.Shared.Func;

namespace AzFunctions.ClientFunctions
{
    public class HandleOrder
    {
        private readonly ILogger<HandleOrder> _logger;
        private readonly IOptions<ConnectionStringSettings> _settings;
        private readonly IOrderRepo _orderRepo;

        public HandleOrder(ILogger<HandleOrder> logger, IOptions<ConnectionStringSettings> settings, IOrderRepo orderRepo)
        {
            _logger = logger;
            _settings = settings;
            _orderRepo = orderRepo;
        }

        // The client function, which is hit by the notification sent to the Pécs subscription in the service bus
        [Function("HandleOrderPecs_Starter")]
        public async Task RunPecs(
                [ServiceBusTrigger(
                    topicName: "%OrdersTopic%",
                    subscriptionName: "%KitchenSubscription_Pecs%",
                    Connection = "ConnectionString",
                    IsSessionsEnabled = true)]
                ServiceBusReceivedMessage message,
                [DurableClient] DurableTaskClient client)
        {
            _logger.LogInformation("An order for the restaurant at Pécs!");

            var msgBody = JsonSerializer.Deserialize<OrderFunctionDto>(message.Body);
            msgBody.City = Cities.Pécs;

            // InstanceId can be stored in the db for example connecting it with the order, and the whole running task can be shut down.
            // This would of course be usefull in case of cancellations or emergency shutdowns.
            string instanceId = await client.ScheduleNewOrchestrationInstanceAsync("Entity_Orchestrator", msgBody);
        }

        // This method could easily be circumvented, by sending the city in the message.
        // This is only implemented to showcase the usage of multiple subscription and hwo the filters work in subscriptions
        [Function("HandleOrderBp_Starter")]
        public async Task RunBp(
                [ServiceBusTrigger(
                    topicName: "%OrdersTopic%",
                    subscriptionName: "%KitchenSubscription_Bp%",
                    Connection = "ConnectionString",
                    IsSessionsEnabled = true)]
                ServiceBusReceivedMessage message,
                [DurableClient] DurableTaskClient client)
        {
            _logger.LogInformation("An order for the restaurant at Budapest!");

            var msgBody = JsonSerializer.Deserialize<OrderFunctionDto>(message.Body);
            msgBody.City = Cities.Budapest;

            string instanceId = await client.ScheduleNewOrchestrationInstanceAsync("HandleOrder_Orchestrator", msgBody);
        }

        // The orchestrator is a classic durable fucntion, which is responsible for the orchestration of the execution of the "small" subtasks
        [Function("HandleOrder_Orchestrator")]
        public static async Task RunOrchestrator([OrchestrationTrigger] TaskOrchestrationContext context)
        {
            ILogger logger = context.CreateReplaySafeLogger("HandleOrder_Orchestrator");
            var inputMsg = context.GetInput<OrderFunctionDto>();

            try
            {
                // To access durable entities, you need to use their type and a unique key (first and second parameter)
                var restaurantEntityId = new EntityInstanceId(
                                                    nameof(Restaurant),
                                                    AzFunctionConstants.RestaurantServiceKey + inputMsg.City.ToInternalString());

                // If the workers count is 0, then the durable entity for the given city is not initialized yet
                var workersCount = await context.Entities.CallEntityAsync<uint>(restaurantEntityId, nameof(Restaurant.GetWorkersCount));
                if (workersCount == 0)
                {
                    await context.Entities.SignalEntityAsync(restaurantEntityId, nameof(Restaurant.InitRestaurant), inputMsg.City);
                }

                // Part of the logic. Different restaurants have different opening times, and orders are only processed, if the order was
                // placed during times, when the restaurant is open.
                var isRestaurantOpen = await context.Entities.CallEntityAsync<bool>(
                                                                    restaurantEntityId,
                                                                    nameof(Restaurant.IsRestaurantOpen),
                                                                    inputMsg.OrderDate);
                if (isRestaurantOpen)
                {
                    //GetMakeTimes for the different pizzas from the db (with a normal AZ function)
                    var pizzasWithBakeTime = await context.CallActivityAsync<IEnumerable<PizzaWithMakeTime>>(nameof(GetMakeTimesAsync), inputMsg);

                    // Would remove from the normal logging. Left it here just to show as an example
                    var tirednessEntityId = new EntityInstanceId(
                                                            nameof(TirednessMonitor),
                                                            AzFunctionConstants.TirednessServiceKey + inputMsg.City.ToInternalString());
                    var tiredness = await context.Entities.CallEntityAsync<double>(tirednessEntityId, nameof(TirednessMonitor.GetTeamTiredness));
                    logger.LogWarning($"Tiredness of the crew before the making of order [{inputMsg.PhoneNumber} - {inputMsg.OrderDate}]: " + tiredness);

                    var bakeTasks = new Task[pizzasWithBakeTime.Count()];
                    int i = 0;
                    foreach (var pizzaToBake in pizzasWithBakeTime)
                    {
                        // The making of each pizza runs simultaniuosly (This part is the most similar to a real life soultuon where we
                        // fan out with 1 function / 1 subtask and then fan in (with the Task.WaitAll)
                        bakeTasks[i] = context.Entities.SignalEntityAsync(restaurantEntityId, nameof(Restaurant.MakePizza), pizzaToBake.TimeToMake);
                        i += 1;
                    }

                    Task.WaitAll(bakeTasks);

                    logger.LogWarning($"All pizzas for order [{inputMsg.PhoneNumber} - {inputMsg.OrderDate}] are ready!");

                    try
                    {
                        // Persist the order completion to the db
                        await context.CallActivityAsync<Task>(nameof(PersistOrderCompletionAsync), inputMsg);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex.Message);
                    }
                }
                else
                {
                    // Cancel order.
                    // Save cancellation into db, like we do completion with the PersistOrderCompletionAsync
                    // Notify user via an event grid notification
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }
        }

        // Reach down to the db and make the db calculate how much time it will take to prepare each pizza connected to the given order
        [Function(nameof(GetMakeTimesAsync))]
        public IEnumerable<PizzaWithMakeTime> GetMakeTimesAsync([ActivityTrigger] OrderFunctionDto input)
        {
            var res = _orderRepo.GetMakeTimes(input.PhoneNumber, input.OrderDate).GetAwaiter().GetResult();
            return res;
        }

        // Update the state of the order (to completed) in the db 
        [Function(nameof(PersistOrderCompletionAsync))]
        public async Task PersistOrderCompletionAsync([ActivityTrigger] OrderFunctionDto order)
        {
            try
            {
                await _orderRepo.CompleteOrder(order.PhoneNumber, order.OrderDate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }
}

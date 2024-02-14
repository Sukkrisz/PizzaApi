using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Infrastructure.Settings;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.DurableTask.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models.Shared.Func;
using Shared.Dto;

namespace AzFunctions
{
    public class EntityFunction
    {
        private readonly ILogger<EntityFunction> _logger;
        private readonly IOptions<ConnectionStringSettings> _settings;

        public EntityFunction(ILogger<EntityFunction> logger, IOptions<ConnectionStringSettings> settings)
        {
            _logger = logger;
            _settings = settings;
        }

        [Function("EntityFunct_Starter")]
        public async Task Run(
                [ServiceBusTrigger(
                                    topicName: "%OrdersTopic%",
                                    subscriptionName: "%KitchenSubscription_Pecs%",
                                    Connection = "ConnectionString",
                                    IsSessionsEnabled = true)] ServiceBusReceivedMessage message,
                [DurableClient] DurableTaskClient client)
        {
            _logger.LogInformation("At least it was hit");

            var msgBody = JsonSerializer.Deserialize<OrderDto>(message.Body);
            var order = new Order(msgBody.PhoneNumber, msgBody.PizzaIds);
            string instanceId = await client.ScheduleNewOrchestrationInstanceAsync("Entity_Orchestrator", order);
        }

        [Function("Entity_Orchestrator")]
        public static async Task<List<string>> RunOrchestrator([OrchestrationTrigger] TaskOrchestrationContext context)
        {
            ILogger logger = context.CreateReplaySafeLogger("Entity_Orchestrator");
            logger.LogInformation("In Orchestrator");

            var outputs = new List<string>();
            /*
            var entityId = new EntityInstanceId(nameof(Counter), "myCounter");

            int currentValue = await context.Entities.CallEntityAsync<int>(entityId, "Get");
            logger.LogInformation("Value: {v}", currentValue);

            await context.Entities.SignalEntityAsync(entityId, "Add", 3);

            currentValue = await context.Entities.CallEntityAsync<int>(entityId, "Get");
            logger.LogInformation("Value: {v}", currentValue);

            await context.Entities.SignalEntityAsync(entityId, "Delete");

            try
            {
                await context.Entities.SignalEntityAsync(entityId, "Add", 3);
                currentValue = await context.Entities.CallEntityAsync<int>(entityId, "Get");
                logger.LogInformation("Value: {v}", currentValue);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }*/


            /*
            var entityId = new EntityInstanceId(nameof(AzStringBuilder), "mySb");
            var sbList = await context.Entities.CallEntityAsync<List<string>>(entityId, "Get");
            logger.LogInformation("Sb list:{x}", sbList.Count);

            await context.Entities.SignalEntityAsync(entityId, "Add", "Hello");
            await context.Entities.SignalEntityAsync(entityId, "Add", "Sleepwalker!");

            sbList = await context.Entities.CallEntityAsync<List<string>>(entityId, "Get");
            logger.LogInformation("Sb list:{x}", sbList.Count);

            foreach (var s in sbList)
            {
                logger.LogInformation(s);
            }

            await context.Entities.SignalEntityAsync(entityId, "Delete");

            try
            {
                await context.Entities.SignalEntityAsync(entityId, "Add", "");
                sbList = await context.Entities.CallEntityAsync<List<string>>(entityId, "Get");
                logger.LogInformation("Value: {v}", sbList is null);
                logger.LogInformation("Value: {v}", sbList?.Count ?? -1);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }*/

            try
            {
                var msg = context.GetInput<OrderDto>();
                var entityId = new EntityInstanceId(nameof(Kitchen1), "myKitchen");
                var orders = await context.Entities.CallEntityAsync<List<int>>(entityId, "GetOrdersToPhoneNumber", msg.PhoneNumber);

                foreach (var id in orders)
                {
                    logger.LogInformation("OrderNum found {id}", id);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }

            return outputs;
        }
    }
}

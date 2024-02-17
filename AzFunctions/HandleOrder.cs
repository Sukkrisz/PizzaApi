using Azure.Messaging.ServiceBus;
using Grpc.Core;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;

namespace AzFunctions
{
    public static class HandleOrder
    {
        [Function("OrderHandler_Orchestrator")]
        public static async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] TaskOrchestrationContext context)
        {
            ILogger logger = context.CreateReplaySafeLogger("OrderHandler_Orchestrator");
            logger.LogInformation("Saying hello.");
            var outputs = new List<string>();

            // Replace name and input with values relevant for your Durable Functions Activity
            outputs.Add(await context.CallActivityAsync<string>(nameof(SayHello), "Tokyo"));
            outputs.Add(await context.CallActivityAsync<string>(nameof(SayHello), "Seattle"));
            outputs.Add(await context.CallActivityAsync<string>(nameof(SayHello), "London"));

            foreach (var output in outputs)
            {
                logger.LogInformation(output);
            }

            // returns ["Hello Tokyo!", "Hello Seattle!", "Hello London!"]
            return outputs;
        }

        [Function(nameof(SayHello))]
        public static string SayHello([ActivityTrigger] string name)
        {
            //ILogger logger = executionContext.GetLogger("SayHello");
            //logger.LogInformation("Saying hello to {name}.", name);
            return $"Hello {name}!";
        }

        /*[Function("HandleOrder_ServiceBusStart")]
        public static async Task<int> HttpStart(
            [ServiceBusTrigger(
                topicName: "%OrdersTopic%",
                subscriptionName: "%KitchenSubscription_Bp%",
                Connection = "ConnectionString",
                IsSessionsEnabled = true)]
            ServiceBusReceivedMessage message,
            [DurableClient] DurableTaskClient client,
            FunctionContext executionContext)
        {
            ILogger logger = executionContext.GetLogger("HandleOrder_ServiceBusStart");

            try
            {
                // Function input comes from the request content.
                //string instanceId = await client.ScheduleNewOrchestrationInstanceAsync("OrderHandler_Orchestrator", message);
                var status = await client.WaitForInstanceCompletionAsync("OrderHandler_Orchestrator");
                if (status.RuntimeStatus == OrchestrationRuntimeStatus.Completed)
                {
                    var result = status.ReadOutputAs<List<string>>();
                    foreach(var r in result)
                    {
                        logger.LogInformation(r);
                    }
                    // Now you can use the result
                }

                logger.LogInformation("Started orchestration with ID = '{instanceId}'.", status.InstanceId);
            }
            catch   (Exception ex)
            {
                logger.LogInformation("Exception: '{exm}'.", ex.Message);
            }
            

            return 1;

            // Returns an HTTP 202 response with an instance management payload.
            // See https://learn.microsoft.com/azure/azure-functions/durable/durable-functions-http-api#start-orchestration
            //return client.CreateCheckStatusResponse(req, instanceId);
        }*/
    }
}

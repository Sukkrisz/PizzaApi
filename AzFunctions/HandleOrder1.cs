using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AzFunctions
{
    public class HandleOrder1
    {
        private readonly ILogger<HandleOrder1> _logger;

        public HandleOrder1(ILogger<HandleOrder1> logger)
        {
            _logger = logger;
        }

        /*[Function("HandleOrderBp_Client")]
        public void HandleOrderBpClient([ServiceBusTrigger(
                                                topicName: "%OrdersTopic%",
                                                subscriptionName: "%KitchenSubscription_Bp%",
                                                Connection = "ConnectionString",
                                                IsSessionsEnabled = true)]
                                        ServiceBusReceivedMessage message,
                                        [DurableClient] IDurableOrchestrationClient context)
        {
            _logger.LogInformation("Bp!");
            _logger.LogInformation("Message SessionId: {id}", message.SessionId);
            _logger.LogInformation("Message ID: {id}", message.MessageId);
            _logger.LogInformation("Message Body: {body}", message.Body);
            _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);
        }*/

        /*[Function("HandleOrderPecs_Client")]
        public void HandleOrderPecsStart([ServiceBusTrigger(
                                    topicName: "%OrdersTopic%",
                                    subscriptionName: "%KitchenSubscription_Pecs%",
                                    Connection = "ConnectionString",
                                    IsSessionsEnabled = true)] ServiceBusReceivedMessage message)
        {
            _logger.LogInformation("Pecs!");
            _logger.LogInformation("Message SessionId: {id}", message.SessionId);
            _logger.LogInformation("Message ID: {id}", message.MessageId);
            _logger.LogInformation("Message Body: {body}", message.Body);
            _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);
        }*/
    }
}

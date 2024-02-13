using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AzFunctions
{
    public class HandleOrder
    {
        private readonly ILogger<HandleOrder> _logger;

        public HandleOrder(ILogger<HandleOrder> logger)
        {
            _logger = logger;
        }

        [Function("HandleOrderBp")]
        public void HandleOrderBp([ServiceBusTrigger(
                                                topicName: "%OrdersTopic%",
                                                subscriptionName: "%KitchenSubscription_Bp%",
                                                Connection = "ConnectionString",
                                                IsSessionsEnabled = true)] ServiceBusReceivedMessage message)
        {
            _logger.LogInformation("Bp!");
            _logger.LogInformation("Message SessionId: {id}", message.SessionId);
            _logger.LogInformation("Message ID: {id}", message.MessageId);
            _logger.LogInformation("Message Body: {body}", message.Body);
            _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);
        }

        [Function("HandleOrderPecs")]
        public void HandleOrderPecs([ServiceBusTrigger(
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
        }
    }
}

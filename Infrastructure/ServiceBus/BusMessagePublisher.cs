using Azure.Messaging.ServiceBus;
using System.Text.Json;

namespace Infrastructure.ServiceBus
{
    public class BusMessagePublisher : IBusMessagePublisher
    {
        private readonly ServiceBusClient _busClient;

        public BusMessagePublisher(ServiceBusClient busClient)
        {
            _busClient = busClient;
        }

        public async Task SendObjectToTopic<T>(T objectToSend, string topicName, IEnumerable<ServiceBusFilter> filters = null)
        {
            var sender = _busClient.CreateSender(topicName);
            using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();

            // Convert the object to a JSON string.
            string jsonText = JsonSerializer.Serialize(objectToSend);
            var message = new ServiceBusMessage(jsonText)
            {
                // Session used to be able to process messages in a FIFO manner
                SessionId = Guid.NewGuid().ToString(),
                ContentType = "application/json"
            };

            // Filters are like metadata. Based on them, the service bus can send data to one topic or another
            if (filters is not null)
            {
                foreach (var filter in filters)
                {
                    message.ApplicationProperties.Add(filter.Key, filter.Value);
                }
            }

            // Send the message to the topic.
            await sender.SendMessageAsync(message);
        }
    }
}

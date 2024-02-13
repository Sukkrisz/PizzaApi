namespace Infrastructure.ServiceBus
{
    public interface IBusMessagePublisher
    {
        Task SendObjectToTopic<T>(T objectToSend, string topicName, IEnumerable<ServiceBusFilter> filters = null);
    }
}

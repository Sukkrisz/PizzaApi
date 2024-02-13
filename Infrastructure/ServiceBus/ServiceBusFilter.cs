namespace Infrastructure.ServiceBus
{
    public class ServiceBusFilter
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public ServiceBusFilter(string filterName, string value)
        {
            Key = filterName;
            Value = value;
        }
    }
}

namespace Infrastructure.Settings
{
    public class AzServiceBusSettings
    {
        public string ConnectionString { get; set; }
        public string OrdersTopic { get; set; }
        public string KitchenSubscription { get; set; }
    }
}

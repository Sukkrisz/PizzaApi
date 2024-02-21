namespace Infrastructure.Settings
{
    // Used for config reading with IOptions
    public class AzServiceBusSettings
    {
        public string ConnectionString { get; set; }
        public string OrdersTopic { get; set; }
        public string KitchenSubscription { get; set; }
    }
}

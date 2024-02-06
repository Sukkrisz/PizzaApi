namespace Data.Db.Network
{
    public class DbSettings
    {
        public ConnectionStringsSettings ConnectionStrings { get; set; }
    }

    public class ConnectionStringsSettings
    {
        public string PizzaDb { get; set; }
    }
}

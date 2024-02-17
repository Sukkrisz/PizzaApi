namespace Models.Shared.Func
{
    [Serializable]
    public class RestaurantEntity
    {
        public uint WorkersCount { get; set; }
        public DateTime? BreakStart { get; set; }
        public uint OpeningHour { get; set; }
        public uint ClosingHour { get; set; }

        public RestaurantEntity()
        {
        }

        public RestaurantEntity(uint workersCount, DateTime? breakStart)
        {
            WorkersCount = workersCount;
            BreakStart = breakStart;
        }
    }
}

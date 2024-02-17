namespace Data.Db.Helpers
{
    internal class DateTimeRounder
    {
        public static DateTime RoundToNearestMinute(DateTime input)
        {
            if (input.Second <= 30)
            {
                return new DateTime(
                    input.Year,
                    input.Month,
                    input.Day,
                    input.Hour,
                    input.Minute,
                    0);
            }
            else
            {
                return new DateTime(
                    input.Year,
                    input.Month,
                    input.Day,
                    input.Hour,
                    input.Minute + 1,
                    0);
            }
        }
    }
}

namespace Database.Helpers
{
    internal sealed class DateTimeRounder
    {
        // In some fields I'm storing smallDateTime types in SQL
        // It only stores untill minutes accuracy, so I rounds them up or down by hand for consistency's sake
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

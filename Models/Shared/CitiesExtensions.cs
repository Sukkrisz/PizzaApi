namespace ModelLibrary.Shared
{
    public static class CitiesExtensions
    {
        public static string ToInternalString(this Cities city)
        {
            switch (city)
            {
                case Cities.Budapest:
                    return "Bp";
                    break;
                case Cities.Pécs:
                    return "Pecs";
                    break;
                default:
                    return string.Empty;
            }
        }
    }
}
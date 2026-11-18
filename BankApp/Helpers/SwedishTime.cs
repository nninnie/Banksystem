namespace BankApp.Helpers
{
    // Helper - Swedish time everywhere, also on Azure (Azure servers run on UTC)
    public static class SwedishTime
    {
        public static DateTime Now =>
            TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Europe/Stockholm");
        public static DateOnly Today => DateOnly.FromDateTime(Now);
    }
}
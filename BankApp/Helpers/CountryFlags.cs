namespace BankApp.Helpers
{
    // Helper – one single place that knows which flag image belongs to which country
    public static class CountryFlags
    {
        // Converts a country name to its two-letter flag code
        private static string Code(string country) => country switch
        {
            "Sweden" => "se",
            "Norway" => "no",
            "Denmark" => "dk",
            "Finland" => "fi",
            _ => "xx" // unknown country
        };

        // Rectangular flag (4:3), used in tables and headers
        public static string Wide(string country) =>
            $"/vendors/flag-icon-css/flags/4x3/{Code(country)}.svg";

        // Square flag (1:1), used under the dashboard chart
        public static string Square(string country) =>
            $"/vendors/flag-icon-css/flags/1x1/{Code(country)}.svg";
    }
}
namespace BankApp.Helpers
{
    // Helper – one single place that knows which flag image belongs to which country
    public static class CountryFlags
    {
        private static string Code(string country) => country switch
        {
            "Sweden" => "se",
            "Norway" => "no",
            "Denmark" => "dk",
            "Finland" => "fi",
            _ => "xx" 
        };

        public static string Wide(string country) =>
            $"/vendors/flag-icon-css/flags/4x3/{Code(country)}.svg";

        public static string Square(string country) =>
            $"/vendors/flag-icon-css/flags/1x1/{Code(country)}.svg";

        public static string IsoCode(string country) =>
            Code(country).ToUpper();
    }
}
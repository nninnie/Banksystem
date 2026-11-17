namespace BankApp.Helpers
{
    // Helper - one single place that knows which currency belongs to which country.
    public static class CountryCurrency
    {
        public static string Code(string? country) => country switch
        {
            "Sweden" => "SEK",
            "Norway" => "NOK",
            "Denmark" => "DKK",
            "Finland" => "EUR",
            _ => ""
        };
    }
}
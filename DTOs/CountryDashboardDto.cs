namespace BankApp.DTOs
{
    // Represents aggregated statistics for a single country
    public class CountryDashboardDto
    {
        public string Country { get; set; }
        public int CustomerCount { get; set; }
        public int AccountCount { get; set; }
        public decimal TotalBalance { get; set; }
       
    }
}

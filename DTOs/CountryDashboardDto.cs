namespace BankApp.DTOs
{
    // Statistik för ETT land
    public class CountryDashboardDto
    {
        public string Country { get; set; }
        public int CustomerCount { get; set; }
        public int AccountCount { get; set; }
        public decimal TotalBalance { get; set; }
       
    }
}

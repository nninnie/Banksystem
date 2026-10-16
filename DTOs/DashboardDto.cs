namespace BankApp.DTOs
{
    // Hela dashboardens data (lista av länder).
    public class DashboardDto
    {
        public List<CountryDashboardDto> Countries { get; set; }
    }
}

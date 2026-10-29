namespace BankApp.DataAccessLayer.DTOs
{
    // Represents the full dashboard data, including the list of countries.
    public class DashboardDto
    {
        public List<CountryDashboardDto> Countries { get; set; }
    }
}

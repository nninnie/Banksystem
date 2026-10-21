using BankApp.DTOs;

namespace BankApp.Services
{
    // Contract for fetching the per-country statistics shown on the start page.
    public interface IIndexService
    {
        Task<List<CountryDashboardDto>> GetCountryStatisticsAsync();
    }
}
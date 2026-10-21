using BankApp.DTOs;

namespace BankApp.Services
{
    public interface IIndexService
    {
        Task<List<CountryDashboardDto>> GetCountryStatisticsAsync();
    }
}
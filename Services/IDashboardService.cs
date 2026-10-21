using BankApp.DTOs;

namespace BankApp.Services
{
    public interface IDashboardService
    {
        Task<DashboardDto> GetDashboardAsync();
    }
}
using BankApp.DataAccessLayer.DTOs;

namespace BankApp.Services.Interfaces
{
    // Contract for fetching all the data the admin dashboard needs in one call.
    public interface IDashboardService
    {
        Task<DashboardDto> GetDashboardAsync();
    }
}
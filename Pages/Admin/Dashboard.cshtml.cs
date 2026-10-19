using BankApp.DTOs;
using BankApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankApp.Pages.Admin
{
    // DashboardModel for the home page. Retrieves dashboard statistics via DashboardService and passes them to the view.
    public class DashboardModel : PageModel
    {
        private readonly DashboardService _dashboardService;

        public DashboardDto Data { get; set; }

        public DashboardModel(DashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }
        public async Task OnGet()
        {
            Data = await _dashboardService.GetDashboardAsync();
        }
    }
}
using BankApp.DataAccessLayer.DTOs;
using BankApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IDashboardService _dashboardService;
        public List<CountryDashboardDto> Countries { get; set; } = new();

        public IndexModel(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public async Task OnGetAsync()
        {
            var data = await _dashboardService.GetDashboardAsync();
            Countries = data.Countries;
        }
    }
}
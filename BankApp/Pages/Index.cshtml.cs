using BankApp.DataAccessLayer.DTOs;
using BankApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BankApp.Helpers;

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

        public DateTime Today { get; set; }

        public async Task OnGetAsync()
        {
            Today = SwedishTime.Now;
            var data = await _dashboardService.GetDashboardAsync();
            Countries = data.Countries;
        }
    }
}
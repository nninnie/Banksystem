using BankApp.DTOs;
using BankApp.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IndexService _indexService;
        public List<CountryDashboardDto> Countries { get; set; } = new();

        public IndexModel(IndexService indexService)
        {
            _indexService = indexService;
        }

        public async Task OnGet()
        {
            Countries = await _indexService.GetCountryStatisticsAsync() ?? new();
        }
    }
}
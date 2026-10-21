using BankApp.DTOs;
using BankApp.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IIndexService _indexService;
        public List<CountryDashboardDto> Countries { get; set; } = new();

        public IndexModel(IIndexService indexService)
        {
            _indexService = indexService;
        }

        public async Task OnGet()
        {
            Countries = await _indexService.GetCountryStatisticsAsync() ?? new();
        }
    }
}
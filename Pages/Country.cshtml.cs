using BankApp.Services;
using BankApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankApp.Pages
{
    [AllowAnonymous]
    [ResponseCache(Duration = 60, VaryByQueryKeys = new[] { "country" })]
    public class CountryModel : PageModel
    {
        private readonly ICustomerService _customerService;

        public CountryModel(ICustomerService customerService)
        {
            _customerService = customerService;
            
        }

        [BindProperty(SupportsGet = true)]
        public string Country { get; set; } = "";

        public List<CustomerViewModel> TopCustomers { get; set; } = new();

        public async Task OnGetAsync()
        {
            TopCustomers = await _customerService.GetTopCustomersByCountryAsync(Country, 10);
        }
    }
}

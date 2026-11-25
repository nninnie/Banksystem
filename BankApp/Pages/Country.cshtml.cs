
using AutoMapper;
using BankApp.Services.Interfaces;
using BankApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace BankApp.Pages
{
    [AllowAnonymous]

    // Caches the response for 60 seconds, separately for each country (?country=...) so the database isn't hit on every page load
    [ResponseCache(Duration = 60, VaryByQueryKeys = new[] { "country" })]
    public class CountryModel : PageModel
    {
        private readonly ICustomerService _customerService;
        private readonly IMapper _mapper;

        public CountryModel(ICustomerService customerService, IMapper mapper)
        {
            _customerService = customerService;
            _mapper = mapper;
        }

        [BindProperty(SupportsGet = true)]
        [StringLength(50)]
        public string Country { get; set; } = "";

        public List<CustomerViewModel> TopCustomers { get; set; } = new();

        // Loads the top 10 customers with the highest total balance in the selected country
        public async Task OnGetAsync()
        {
            if (!ModelState.IsValid) return;

            var customers = await _customerService
                .GetTopCustomersByCountryAsync(Country, 10);

            TopCustomers = _mapper.Map<List<CustomerViewModel>>(customers);
        }
    }
}


using AutoMapper;
using BankApp.Services.Interfaces;
using BankApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace BankApp.Pages
{
    // The page that shows the top customers in a given country
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

        // The country comes from the URL, e.g. /Country?country=Sweden
        // SupportsGet = true makes binding work on GET requests, not just POST
        [BindProperty(SupportsGet = true)]
        [StringLength(50)]
        public string Country { get; set; } = "";

        public List<CustomerViewModel> TopCustomers { get; set; } = new();

        public async Task OnGetAsync()
        {
            if (!ModelState.IsValid) return;

            var customers = await _customerService
                .GetTopCustomersByCountryAsync(Country, 10);

            TopCustomers = _mapper.Map<List<CustomerViewModel>>(customers);
        }
    }
}

using BankApp.Services;
using BankApp.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankApp.Pages.Admin
{

    // VeiwModel - for the customer list: handles searching, pagination, and displaying customers in the admin view

    public class CustomersModel : PageModel
    {
        private readonly CustomerService _service;

        private readonly IMapper _mapper;

        public CustomersModel(CustomerService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        public List<CustomerViewModel> Customers { get; set; }

        public async Task OnGetAsync(string searchName, string searchCity)
        {
            var customers = await _service.SearchCustomersAsync(searchName, searchCity);

            Customers = _mapper.Map<List<CustomerViewModel>>(customers);
        }

    }

}

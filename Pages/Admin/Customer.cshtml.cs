using AutoMapper;
using BankApp.Services;
using BankApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankApp.Pages.Admin
{
    // PageModel – handles the logic for the Customer page
    public class CustomerModel : PageModel
    {
        private readonly CustomerService _service;
        private readonly IMapper _mapper;
        public CustomerViewModel Customer { get; set; }
        
        public CustomerModel(CustomerService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        // Handles the GET request for the Customer page.
        // Fetches the customer by ID
        // Return notfound if customer is not found
        // Maps the entity to a ViewModel
        // Loads all accounts for the customer
        // Maps accounts to AccountViewModels
        // Calculates total balance
        // Returns the page with the filled ViewModel
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var customer = await _service.GetCustomerAsync(id);

            if (customer == null)
                return NotFound();

            Customer = _mapper.Map<CustomerViewModel>(customer);

            var accounts = await _service.GetAccountsAsync(customer);

            Customer.Accounts = _mapper.Map<List<AccountViewModel>>(accounts);

            Customer.TotalBalance = await _service.GetTotalBalanceAsync(customer);

            return Page();
        }
    }
}

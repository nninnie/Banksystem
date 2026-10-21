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
        private readonly ICustomerService _customerService;
        private readonly IMapper _mapper;
        public CustomerViewModel Customer { get; set; }
        
        public CustomerModel(ICustomerService customerService, IMapper mapper)
        {
            _customerService = customerService;
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
            var customer = await _customerService.GetCustomerAsync(id);

            if (customer == null)
                return NotFound();

            Customer = _mapper.Map<CustomerViewModel>(customer);

            var accounts = await _customerService.GetAccountsAsync(customer);

            Customer.Accounts = _mapper.Map<List<AccountViewModel>>(accounts);

            Customer.TotalBalance = await _customerService.GetTotalBalanceAsync(customer);

            return Page();
        }
    }
}

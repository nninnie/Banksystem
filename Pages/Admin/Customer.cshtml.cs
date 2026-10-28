using AutoMapper;
using BankApp.Services.Services;
using BankApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace BankApp.Pages.Admin
{
    // PageModel – handles the logic for the Customer page
    [Authorize(Roles = "Admin,Cashier")]
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

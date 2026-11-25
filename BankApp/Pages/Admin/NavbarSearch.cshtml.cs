using BankApp.Services.Interfaces;
using BankApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankApp.Pages.Admin
{
    // PageModel - quick navbar search: find a customer by customer number
    [Authorize(Roles = "Admin,Cashier")]
    public class NavbarSearchModel : PageModel
    {
        private readonly ICustomerService _customerService;

        public NavbarSearchModel(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [BindProperty(SupportsGet = true)]
        public NavbarSearchViewModel Search { get; set; } = new();

        // Looks up a customer by number from the navbar search, then opens the customer page.
        public async Task<IActionResult> OnGetAsync()
        {
            if (!Request.Query.ContainsKey("Search.CustomerId"))
            {
                ModelState.Clear();
                return Page();
            }

            if (!ModelState.IsValid)
                return Page();

            var customer = await _customerService.GetCustomerAsync(Search.CustomerId!.Value);

            if (customer == null)
            {
                ModelState.AddModelError("Search.CustomerId", $"No customer with number {Search.CustomerId} was found.");
                return Page();
            }

            return RedirectToPage("/Admin/Customer", new { id = Search.CustomerId });
        }
    }
}
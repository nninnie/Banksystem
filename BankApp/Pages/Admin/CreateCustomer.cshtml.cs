using AutoMapper;
using BankApp.DataAccessLayer.Models;
using BankApp.Services.Interfaces;
using BankApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankApp.Pages.Admin
{
    // PageModel – create a new customer (Cashier only)
    [Authorize(Roles = "Cashier")]
    public class CreateCustomerModel : PageModel
    {
        private readonly ICustomerService _customerService;
        private readonly IMapper _mapper;

        public CreateCustomerModel(ICustomerService customerService, IMapper mapper)
        {
            _customerService = customerService;
            _mapper = mapper;
        }

        // Typed in the form, filled automatically on POST
        [BindProperty]
        public CustomerFormViewModel Input { get; set; } = new();

        // GET – page opens with an empty form
        public void OnGet()
        {
        }

        // POST – form sent: validate, create customer + account, then go to the customer page
        public async Task<IActionResult> OnPostAsync()
        {
            // Extra rule that DataAnnotations can not check
            if (Input.Birthday > DateOnly.FromDateTime(DateTime.Today))
                ModelState.AddModelError("Input.Birthday", "Birthday can not be in the future.");

            // Server-side validation
            if (!ModelState.IsValid)
                return Page();

            var customer = _mapper.Map<Customer>(Input);
            var newCustomerId = await _customerService.CreateCustomerAsync(customer);

            return RedirectToPage("/Admin/Customer", new { id = newCustomerId });
        }
    }
}
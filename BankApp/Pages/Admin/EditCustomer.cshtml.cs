using AutoMapper;
using BankApp.DataAccessLayer.Models;
using BankApp.Services.Interfaces;
using BankApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BankApp.Helpers;

namespace BankApp.Pages.Admin
{
    // PageModel – edit an existing customer (Cashier only)
    [Authorize(Roles = "Cashier")]
    public class EditCustomerModel : PageModel
    {
        private readonly ICustomerService _customerService;
        private readonly IMapper _mapper;

        public EditCustomerModel(ICustomerService customerService, IMapper mapper)
        {
            _customerService = customerService;
            _mapper = mapper;
        }

        // Filled with the customer's current values on GET, typed in the form on POST
        [BindProperty]
        public CustomerFormViewModel Input { get; set; } = new();

        // GET – page opens: load the customer and fill the form
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var customer = await _customerService.GetCustomerAsync(id);

            if (customer == null)
                return NotFound();

            Input = _mapper.Map<CustomerFormViewModel>(customer);

            return Page();
        }

        // POST – form sent: validate, update, then go back to the customer page
        public async Task<IActionResult> OnPostAsync(int id)
        {
            // The id in the address is the truth, not the hidden field in the form
            Input.CustomerId = id;

            // Extra rule that DataAnnotations can not check
            if (Input.Birthday > SwedishTime.Today)
                ModelState.AddModelError("Input.Birthday", "Birthday can not be in the future.");

            // Server-side validation
            if (!ModelState.IsValid)
                return Page();

            var customer = _mapper.Map<Customer>(Input);
            var found = await _customerService.UpdateCustomerAsync(customer);

            if (!found)
                return NotFound();

            return RedirectToPage("/Admin/Customer", new { id });
        }
    }
}
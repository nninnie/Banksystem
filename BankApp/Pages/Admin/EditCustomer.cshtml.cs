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
    // PageModel – edit an existing customer 
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
            Input.CustomerId = id;

            if (Input.Birthday > SwedishTime.Today)
                ModelState.AddModelError("Input.Birthday", "Birthday can not be in the future.");

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
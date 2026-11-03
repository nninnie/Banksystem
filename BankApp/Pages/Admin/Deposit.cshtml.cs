using AutoMapper;
using BankApp.Services.Enums;
using BankApp.Services.Interfaces;
using BankApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankApp.Pages.Admin
{
    // PageModel – deposit money to one account (Cashier only)
    [Authorize(Roles = "Cashier")]
    public class DepositModel : PageModel
    {
        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;

        public DepositModel(IAccountService accountService, IMapper mapper)
        {
            _accountService = accountService;
            _mapper = mapper;
        }

        // Shown on the page: owner, account number, balance
        public AccountViewModel Account { get; set; } = new();

        // Typed in the form, filled automatically on POST
        [BindProperty]
        public DepositWithdrawViewModel Input { get; set; } = new();


        // GET – page opens: load the account and show it
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var account = await _accountService.GetAccountAsync(id);

            if (account == null)
                return NotFound();

            Account = _mapper.Map<AccountViewModel>(account);
            return Page();
        }

        // POST – form sent: validate, deposit, then redirect or show errors
        public async Task<IActionResult> OnPostAsync(int id)
        {
            var account = await _accountService.GetAccountAsync(id);
            if (account == null)
                return NotFound();

            Account = _mapper.Map<AccountViewModel>(account);

            // Server-side validation
            if (!ModelState.IsValid)
                return Page();

            var result = await _accountService.DepositAsync(id, Input.Amount);

            if (result == ErrorCode.OK)
                return RedirectToPage("/Admin/Account", new { id });

            if (result == ErrorCode.IncorrectAmount)
                ModelState.AddModelError("Input.Amount", "Amount must be greater than 0.");

            return Page();
        }
    }
}
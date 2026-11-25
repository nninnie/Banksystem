using AutoMapper;
using BankApp.Services.Enums;
using BankApp.Services.Interfaces;
using BankApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankApp.Pages.Admin
{
    // PageModel – deposit money to one account 
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

        public AccountViewModel Account { get; set; } = new();
        public List<TransactionViewModel> RecentTransactions { get; set; } = new();

        [BindProperty]
        public DepositWithdrawViewModel Input { get; set; } = new();

        // Loads the account and its 5 latest transactions for the form page.
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var account = await _accountService.GetAccountAsync(id);

            if (account == null)
                return NotFound();

            Account = _mapper.Map<AccountViewModel>(account);

            var transactions = await _accountService.GetTransactionsAsync(id, 0, 5);
            RecentTransactions = _mapper.Map<List<TransactionViewModel>>(transactions);

            return Page();
        }

        // POST – form sent: validate, deposit, then redirect or show errors
        public async Task<IActionResult> OnPostAsync(int id)
        {
            var account = await _accountService.GetAccountAsync(id);
            if (account == null)
                return NotFound();

            Account = _mapper.Map<AccountViewModel>(account);

            var transactions = await _accountService.GetTransactionsAsync(id, 0, 5);
            RecentTransactions = _mapper.Map<List<TransactionViewModel>>(transactions);

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
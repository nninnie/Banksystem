using AutoMapper;
using BankApp.Services.Enums;
using BankApp.Services.Interfaces;
using BankApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankApp.Pages.Admin
{
    // PageModel – withdraw money from one account 
    [Authorize(Roles = "Cashier")]
    public class WithdrawModel : PageModel
    {
        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;

        public WithdrawModel(IAccountService accountService, IMapper mapper)
        {
            _accountService = accountService;
            _mapper = mapper;
        }

        public AccountViewModel Account { get; set; } = new();

        public List<TransactionViewModel> RecentTransactions { get; set; } = new();

        [BindProperty]
        public DepositWithdrawViewModel Input { get; set; } = new();

        // GET – page opens: load the account and show it
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

        // POST – form sent: validate, withdraw, then redirect or show errors
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

            var result = await _accountService.WithdrawAsync(id, Input.Amount);

            if (result == ErrorCode.OK)
            {
                if (_accountService.IsLargeWithdrawal(Input.Amount))
                    TempData["LargeWithdrawal"] = true;

                return RedirectToPage("/Admin/Account", new { id });
            }

            if (result == ErrorCode.IncorrectAmount)
                ModelState.AddModelError("Input.Amount", "Amount must be greater than 0.");

            if (result == ErrorCode.BalanceTooLow)
                ModelState.AddModelError("Input.Amount", "Insufficient funds. The amount is higher than the current balance.");

            return Page();
        }
    }
}
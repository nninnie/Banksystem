using AutoMapper;
using BankApp.Services.Enums;
using BankApp.Services.Interfaces;
using BankApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankApp.Pages.Admin
{
    // PageModel – transfer money from one account to another (Cashier only)
    [Authorize(Roles = "Cashier")]
    public class TransferModel : PageModel
    {
        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;

        public TransferModel(IAccountService accountService, IMapper mapper)
        {
            _accountService = accountService;
            _mapper = mapper;
        }

        // The sending account: owner, account number, balance
        public AccountViewModel Account { get; set; } = new();

        // The 5 latest transactions, shown under the form
        public List<TransactionViewModel> RecentTransactions { get; set; } = new();

        // Typed in the form, filled automatically on POST
        [BindProperty]
        public TransferViewModel Input { get; set; } = new();


        // GET – page opens: load the sending account and show it
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

        // POST – form sent: validate, transfer, then redirect or show errors
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

            var result = await _accountService.TransferAsync(id, Input.ToAccountId!.Value, Input.Amount);

            if (result == ErrorCode.OK)
            {
                TempData["TransferCompleted"] = true;
                return RedirectToPage("/Admin/Account", new { id });
            }

            if (result == ErrorCode.IncorrectAmount)
                ModelState.AddModelError("Input.Amount", "Amount must be greater than 0.");

            if (result == ErrorCode.BalanceTooLow)
                ModelState.AddModelError("Input.Amount", "Insufficient funds. The amount is higher than the current balance.");

            if (result == ErrorCode.AccountNotFound)
                ModelState.AddModelError("Input.ToAccountId", "The receiving account does not exist.");

            if (result == ErrorCode.SameAccount)
                ModelState.AddModelError("Input.ToAccountId", "You can't transfer to the same account.");

            return Page();
        }
    }
}

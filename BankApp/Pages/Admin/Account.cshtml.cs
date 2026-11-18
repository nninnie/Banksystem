using AutoMapper;
using BankApp.Services.Interfaces;
using BankApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankApp.Pages.Admin
{
    // PageModel – shows one account and loads its transactions with AJAX
    [Authorize(Roles = "Admin,Cashier")]
    public class AccountModel : PageModel
    {
        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;

        public AccountViewModel Account { get; set; }

        public int TransactionCount { get; set; }

        public AccountModel(IAccountService accountService, IMapper mapper)
        {
            _accountService = accountService;
            _mapper = mapper;

        }

        // Shows account number and balance. Transactions are loaded by JavaScript.
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var account = await _accountService.GetAccountAsync(id);

            if (account == null)
                return NotFound();

            Account = _mapper.Map<AccountViewModel>(account);
            TransactionCount = await _accountService.GetTransactionCountAsync(id);

            return Page();
        }

        // AJAX handler – returns 20 transactions per page as JSON
        public async Task<IActionResult> OnGetShowMoreAsync(int id, int pageNo)
        {
            if (pageNo < 1) pageNo = 1; // Protects against negative Skip

            var transactions = await _accountService.GetTransactionsAsync(id, (pageNo - 1) * 20, 20);

            var result = _mapper.Map<List<TransactionViewModel>>(transactions);

            return new JsonResult(new { transactions = result });
        }
    }
}
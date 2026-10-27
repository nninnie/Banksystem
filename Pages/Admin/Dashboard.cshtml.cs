using BankApp.DataAccessLayer.DTOs;
using BankApp.Services.Services;
using BankApp.ViewModels;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankApp.Pages.Admin
{
    // DashboardModel for the home page. Retrieves dashboard statistics via DashboardService and passes them to the view.
    public class DashboardModel : PageModel
    {
        private readonly IDashboardService _dashboardService;

        private readonly ITransactionService _transactionService;

        public DashboardDto Data { get; set; }
        public List<TransactionViewModel> RecentTransactions { get; set; } = new();

        public string DisplayName { get; set; } = "";

        public string Greeting { get; set; } = "";

        public DashboardModel(IDashboardService dashboardService, ITransactionService transactionService)
        {
            _dashboardService = dashboardService;
            _transactionService = transactionService;
        }
        public async Task OnGet()
        {
            Data = await _dashboardService.GetDashboardAsync();


            var transactions = await _transactionService.GetRecentTransactionsAsync(5);

            RecentTransactions = transactions.Select(t => new TransactionViewModel
            {
                TransactionId = t.TransactionId,
                AccountId = t.AccountId,
                CustomerName = t.CustomerName,
                Date = t.Date,
                Type = t.Type,
                Operation = t.Operation ?? "",
                Amount = t.Amount,
                Balance = t.Balance
            }).ToList();


            var hour = DateTime.Now.Hour;

            Greeting = hour switch
            {
                >= 5 and < 12 => "Good morning",
                >= 12 and < 18 => "Good afternoon",
                _ => "Good evening"
            };
        }
    }
}
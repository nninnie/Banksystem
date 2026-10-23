using BankApp.DTOs;
using BankApp.Services;
using BankApp.ViewModels;
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
            RecentTransactions = await _transactionService.GetRecentTransactionsAsync(5);

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
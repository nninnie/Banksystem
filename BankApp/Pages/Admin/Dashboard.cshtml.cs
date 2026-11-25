using AutoMapper;
using BankApp.DataAccessLayer.DTOs;
using BankApp.Helpers;
using BankApp.Services.Interfaces;
using BankApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankApp.Pages.Admin
{
    // Admin dashboard: shows recent transactions, a greeting and the country statistics.
    [Authorize(Roles = "Admin")]
    public class DashboardModel : PageModel
    {
        private readonly IDashboardService _dashboardService;

        private readonly ITransactionService _transactionService;

        public DashboardDto Data { get; set; }
        public List<TransactionViewModel> RecentTransactions { get; set; } = new();

        public string DisplayName { get; set; } = "";

        public string Greeting { get; set; } = "";
        public DateTime Today { get; set; }

        private readonly IMapper _mapper;

        public DashboardModel(IDashboardService dashboardService, ITransactionService transactionService, IMapper mapper)
        {
            _dashboardService = dashboardService;
            _transactionService = transactionService;
            _mapper = mapper;
        }

        // Loads the admin dashboard: country statistics, the latest transactions and a greeting
        public async Task OnGet()
        {
            Data = await _dashboardService.GetDashboardAsync();


            var transactions = await _transactionService.GetRecentTransactionsAsync(5);

            RecentTransactions = _mapper.Map<List<TransactionViewModel>>(transactions);

            Today = SwedishTime.Now;
            var hour = Today.Hour;

            Greeting = hour switch
            {
                >= 5 and < 12 => "Good morning",
                >= 12 and < 18 => "Good afternoon",
                _ => "Good evening"
            };

            DisplayName = UserDisplay.Name(User);
        }
    }
}
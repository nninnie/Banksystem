using BankApp.Models;
using BankApp.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BankApp.Services
{
    // Fetches transaction data from the database and turns it into view models for the pages.
    public class TransactionService : ITransactionService
    {
        private readonly BankAppContext _context;

        public TransactionService(BankAppContext context)
        {
            _context = context;
        }

        public async Task<List<TransactionViewModel>> GetRecentTransactionsAsync(int count)
        {
            return await _context.Transactions
        .AsNoTracking()
        .OrderByDescending(t => t.Date)
        .ThenByDescending(t => t.TransactionId)
        .Take(count)
        .Select(t => new TransactionViewModel
        {
            TransactionId = t.TransactionId,
            AccountId = t.AccountId,
            CustomerName = t.AccountNavigation.Dispositions
                .Select(d => d.Customer.Givenname + " " + d.Customer.Surname)
                .FirstOrDefault() ?? "Unknown",
            Type = t.Type,
            Operation = t.Operation,
            Amount = t.Amount,
            Balance = t.Balance,
            Date = t.Date.ToDateTime(TimeOnly.MinValue)
        })
        .ToListAsync();
        }
    }
}

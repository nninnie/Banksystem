using BankApp.DataAccessLayer.Data;
using BankApp.DataAccessLayer.DTOs;
using BankApp.DataAccessLayer.Models;
using BankApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BankApp.Services.Services
{
    // Fetches transaction data from the database and turns it into view models for the pages.
    public class TransactionService : ITransactionService
    {
        private readonly BankAppContext _context;

        public TransactionService(BankAppContext context)
        {
            _context = context;
        }

        // Loads the 5 latest transactions and their owner names using separate, lightweight SQL queries to prevent timeouts; 
        // it then maps the data using a standard in-memory .ToList() because the SQL database has already finished its work.
        public async Task<List<TransactionDto>> GetRecentTransactionsAsync(int count)
        {
            var recent = await _context.Transactions
                .AsNoTracking()
                .OrderByDescending(t => t.TransactionId)
                .Take(count)
                .Select(t => new
                {
                    t.TransactionId,
                    t.AccountId,
                    t.Type,
                    t.Operation,
                    t.Amount,
                    t.Balance,
                    t.Date
                })
                .ToListAsync();

            var accountIds = recent.Select(r => r.AccountId).Distinct().ToList();

            var owners = await _context.Dispositions
                .AsNoTracking()
                .Where(d => accountIds.Contains(d.AccountId))
                .OrderBy(d => d.DispositionId)
                .Select(d => new
                {
                    d.AccountId,
                    Name = d.Customer.Givenname + " " + d.Customer.Surname
                })
                .ToListAsync();

            var nameByAccount = owners
                .GroupBy(o => o.AccountId)
                .ToDictionary(g => g.Key, g => g.First().Name);

            return recent.Select(t => new TransactionDto
            {
                TransactionId = t.TransactionId,
                AccountId = t.AccountId,
                CustomerName = nameByAccount.GetValueOrDefault(t.AccountId) ?? "Unknown",
                Type = t.Type,
                Operation = t.Operation,
                Amount = t.Amount,
                Balance = t.Balance,
                Date = t.Date.ToDateTime(TimeOnly.MinValue)
            }).ToList();
        }
    }
}

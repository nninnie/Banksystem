using BankApp.DataAccessLayer.DTOs;
using BankApp.DataAccessLayer.Models;
using BankApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BankApp.Services.Services
{
    // Service - Rules for finding suspicious transactions
    public class SuspiciousTransactionService : ISuspiciousTransactionService
    {
        private const decimal SingleTransactionLimit = 15000m;

        private const decimal TotalLimit = 23000m;
        private const int DaysBack = 3;

        private readonly BankAppContext _context;

        public SuspiciousTransactionService(BankAppContext context)
        {
            _context = context;
        }

        // Every country that has customers
        public async Task<List<string>> GetCountriesAsync()
        {
            return await _context.Customers
                .Select(c => c.Country)
                .Distinct()
                .ToListAsync();
        }

        // Highest transaction id, saved after each run
        public async Task<int> GetLastTransactionIdAsync()
        {
            return await _context.Transactions.MaxAsync(t => (int?)t.TransactionId) ?? 0;
        }

        // Runs both rules, only for transactions after the last run
        public async Task<List<SuspiciousTransactionDto>> FindSuspiciousAsync(string country, int afterTransactionId)
        {
            var single = await FindSingleLargeAsync(country, afterTransactionId);
            var total = await FindHighTotalAsync(country, afterTransactionId);

            // One transaction can break both rules, so show it once
            return single.Concat(total)
                .GroupBy(s => s.TransactionId)
                .Select(g =>
                {
                    var first = g.First();
                    first.Reason = string.Join(" + ", g.Select(x => x.Reason));
                    return first;
                })
                .OrderBy(s => s.TransactionId)
                .ToList();
        }

        // Single large transaction - Start in Transactions (few rows), then join to customer
        private async Task<List<SuspiciousTransactionDto>> FindSingleLargeAsync(string country, int afterTransactionId)
        {
            // OWNER only, so shared accounts are not counted twice
            return await _context.Transactions
                .Where(t => t.TransactionId > afterTransactionId
                         && (t.Amount > SingleTransactionLimit || t.Amount < -SingleTransactionLimit))
                .SelectMany(t => t.AccountNavigation.Dispositions
                    .Where(d => d.Type == "OWNER" && d.Customer.Country == country)
                    .Select(d => new SuspiciousTransactionDto
                    {
                        Country = country,
                        CustomerId = d.CustomerId,
                        CustomerName = d.Customer.Givenname + " " + d.Customer.Surname,
                        AccountId = d.AccountId,
                        TransactionId = t.TransactionId,
                        Date = t.Date,
                        Amount = t.Amount,
                        Reason = "Single transaction over 15 000"
                    }))
                .ToListAsync();
        }

        // High total - A customer's sum over the last 3 days
        private async Task<List<SuspiciousTransactionDto>> FindHighTotalAsync(string country, int afterTransactionId)
        {
            // Date has no time of day, so 72 hours = 3 days
            var since = DateOnly.FromDateTime(DateTime.Now.AddDays(-DaysBack));

            // All transactions in the window, old ones count in the total too
            var recent = await _context.Transactions
                .Where(t => t.Date >= since)
                .SelectMany(t => t.AccountNavigation.Dispositions
                    .Where(d => d.Type == "OWNER" && d.Customer.Country == country)
                    .Select(d => new SuspiciousTransactionDto
                    {
                        Country = country,
                        CustomerId = d.CustomerId,
                        CustomerName = d.Customer.Givenname + " " + d.Customer.Surname,
                        AccountId = d.AccountId,
                        TransactionId = t.TransactionId,
                        Date = t.Date,
                        Amount = t.Amount,
                        Reason = "Total over 23 000 in 3 days"
                    }))
                .ToListAsync();

            // Sum per customer, keep only the new transactions
            return recent
                .GroupBy(r => r.CustomerId)
                .Where(g => g.Sum(r => Math.Abs(r.Amount)) > TotalLimit)
                .SelectMany(g => g)
                .Where(r => r.TransactionId > afterTransactionId)
                .ToList();
        }
    }
}
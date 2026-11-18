using BankApp.DataAccessLayer.DTOs;
using BankApp.DataAccessLayer.Models;
using BankApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BankApp.Services.Services
{
    // Service - Rules for finding suspicious transactions
    public class SuspiciousTransactionService : ISuspiciousTransactionService
    {
        // Rule limits
        private const decimal SingleTransactionLimit = 15000m;
        private const decimal TotalLimit = 23000m;
        private const int DaysBack = 2;

        private readonly BankAppContext _context;

        // The owner of an account 
        private record Owner(int CustomerId, string CustomerName);

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
        public async Task<List<SuspiciousTransactionDto>> FindSuspiciousAsync(string country, int afterTransactionId, DateOnly? since = null)
        {
            var from = since ?? DateOnly.MinValue;
            var owners = await GetOwnersAsync(country);

            var single = await FindSingleLargeAsync(country, afterTransactionId, from, owners);
            var total = await FindHighTotalAsync(country, afterTransactionId, from, owners);

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

        // Account owners in one country - a small query, so it is fast
        private async Task<Dictionary<int, Owner>> GetOwnersAsync(string country)
        {
            var list = await _context.Dispositions
                .Where(d => d.Type == "OWNER" && d.Customer.Country == country)
                .Select(d => new
                {
                    d.AccountId,
                    d.CustomerId,
                    Name = d.Customer.Givenname + " " + d.Customer.Surname
                })
                .ToListAsync();

            // AccountId is the key, so a transaction finds its owner directly
            return list
                .GroupBy(o => o.AccountId)
                .ToDictionary(g => g.Key, g => new Owner(g.First().CustomerId, g.First().Name));
        }

        // Single large transaction - Only the matching rows, joined to the owner in memory
        private async Task<List<SuspiciousTransactionDto>> FindSingleLargeAsync(
            string country, int afterTransactionId, DateOnly from, Dictionary<int, Owner> owners)
        {
            var accountIds = owners.Keys.ToList();

            var large = await _context.Transactions
                .Where(t => t.TransactionId > afterTransactionId
                         && t.Date >= from
                         && accountIds.Contains(t.AccountId)
                         && (t.Amount > SingleTransactionLimit || t.Amount < -SingleTransactionLimit))
                .Select(t => new { t.TransactionId, t.AccountId, t.Date, t.Amount })
                .ToListAsync();

            return large
                .Select(t => new SuspiciousTransactionDto
                {
                    Country = country,
                    CustomerId = owners[t.AccountId].CustomerId,
                    CustomerName = owners[t.AccountId].CustomerName,
                    AccountId = t.AccountId,
                    TransactionId = t.TransactionId,
                    Date = t.Date,
                    Amount = t.Amount,
                    Reason = "Single transaction over 15 000"
                })
                .ToList();
        }

        // High total - A customer's sum over the last 3 days
        private async Task<List<SuspiciousTransactionDto>> FindHighTotalAsync(
            string country, int afterTransactionId, DateOnly from, Dictionary<int, Owner> owners)
        {
            // Date has no time of day, so 72 hours = 3 days
            var since = DateOnly.FromDateTime(DateTime.Now.AddDays(-DaysBack));

            // All transactions in the window, old ones count in the total too
            var recent = await _context.Transactions
            .Where(t => t.Date >= since && t.Date >= from
             && _context.Dispositions.Any(d => d.AccountId == t.AccountId
                                           && d.Type == "OWNER"
                                           && d.Customer.Country == country))
            .Select(t => new { t.TransactionId, t.AccountId, t.Date, t.Amount })
            .ToListAsync();

            var rows = recent.Select(t => new SuspiciousTransactionDto
            {
                Country = country,
                CustomerId = owners[t.AccountId].CustomerId,
                CustomerName = owners[t.AccountId].CustomerName,
                AccountId = t.AccountId,
                TransactionId = t.TransactionId,
                Date = t.Date,
                Amount = t.Amount,
                Reason = "Total over 23 000 in 3 days"
            });

            // Sum per customer, keep only the new transactions
            return rows
                .GroupBy(r => r.CustomerId)
                .Where(g => g.Sum(r => Math.Abs(r.Amount)) > TotalLimit)
                .SelectMany(g => g)
                .Where(r => r.TransactionId > afterTransactionId)
                .ToList();
        }
    }
}
using BankApp.DTOs;
using BankApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BankApp.Services
{
    // Calculates per-country statistics (customers, accounts, total balance) for the start page.
    public class IndexService : IIndexService
    {
        private readonly BankAppContext _context;

        public IndexService(BankAppContext context)
        {
            _context = context;
        }

        public async Task<List<CountryDashboardDto>> GetCountryStatisticsAsync()
        {
            return await _context.Customers
                .GroupBy(c => c.Country)
                .Select(g => new CountryDashboardDto
                {
                    Country = g.Key,
                    CustomerCount = g.Count(),
                    AccountCount = g.SelectMany(c => c.Dispositions).Count(),
                    TotalBalance = g.SelectMany(c => c.Dispositions)
                                    .Select(d => d.Account.Balance)
                                    .Sum()
                })
                .ToListAsync();
        }
    }
}

using BankApp.DTOs;
using BankApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BankApp.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly BankAppContext _context;

        public DashboardService(BankAppContext context)
        {
            _context = context;
        }

        // Builds dashboard statistics by grouping customers per country directly in the database
        public async Task<DashboardDto> GetDashboardAsync()
        {
            var countries = await _context.Customers
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

            return new DashboardDto
            {
                Countries = countries
            };
        }
    }
}

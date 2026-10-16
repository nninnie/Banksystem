using BankApp.DTOs;
using BankApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BankApp.Services
{
    public class DashboardService
    {
        private readonly BankAppContext _context;

        public DashboardService(BankAppContext context)
        {
            _context = context;
        }

        // Metoden hämtar alla kunder med konton, grupperar dem i minnet och bygger dashboardens statistik.
        public async Task<DashboardDto> GetDashboardAsync()
        {
            var customers = await _context.Customers
                .Include(c => c.Dispositions)
                .ThenInclude(d => d.Account)
                .ToListAsync();

            var countries = await _context.Customers
                .GroupBy(c => c.Country)
                .Select(g => new CountryDashboardDto
                {
                    Country = g.Key,
                    CustomerCount = g.Count(),
                    AccountCount = g.SelectMany(c => c.Dispositions).Count(),
                    TotalBalance = g.SelectMany(c => c.Dispositions).Select(d => d.Account).Sum(a => a.Balance)
                })
                .ToListAsync();

            return new DashboardDto
            {
                Countries = countries
            };

        }
    }
}

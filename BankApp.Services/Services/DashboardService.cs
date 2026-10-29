using BankApp.DataAccessLayer.Data;
using BankApp.DataAccessLayer.DTOs;
using BankApp.DataAccessLayer.Models;
using BankApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BankApp.Services.Services
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
                    // Only OWNER dispositions – otherwise shared accounts are counted twice
                    AccountCount = g.SelectMany(c => c.Dispositions)
                    .Count(d => d.Type == "OWNER"),
                    TotalBalance = g.SelectMany(c => c.Dispositions)
                    .Where(d => d.Type == "OWNER")
                    .Sum(d => d.Account.Balance)
                })
                .ToListAsync();


            return new DashboardDto
            {
                Countries = countries
            };
        }
    }
}

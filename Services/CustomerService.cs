using BankApp.DTOs;
using BankApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BankApp.Services
{
    // Service class – handles all customer-related data logic
    public class CustomerService
    {
        private readonly BankAppContext _context;

        public CustomerService(BankAppContext context)
        {
            _context = context;
        }

        // Async method – fetches a customer by ID including accounts
        public async Task<Customer?> GetCustomerAsync(int id)
        {
            return await _context.Customers
                .Include(c => c.Dispositions)
                .ThenInclude(d => d.Account)
                .FirstOrDefaultAsync(c => c.CustomerId == id);
        }


        // Async method – calculates the customer's total balance
        public async Task<decimal> GetTotalBalanceAsync(Customer customer)
        {
            return await Task.FromResult(
                customer.Dispositions
                .Where(d => d.Account != null)
                .Sum(d => d.Account.Balance));
        }


        // Async method – returns all accounts belonging to the customer
        public async Task<List<Account>> GetAccountsAsync(Customer customer)
        {
            return await Task.FromResult(
                customer.Dispositions
                .Where(d => d.Account != null)
                .Select(d => d.Account)
                .ToList()
                );
        }

        public async Task<List<Customer>> SearchCustomersAsync(string name, string city)
        {
            var query = _context.Customers.AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(c => c.Givenname.Contains(name) ||
                                         c.Surname.Contains(name));
            }

            if (!string.IsNullOrWhiteSpace(city))
            {
                query = query.Where(c => c.City.Contains(city));
            }

            return await query
                .OrderBy(c => c.CustomerId)
                .Take(50) // första 50 (paginering kommer senare)
                .ToListAsync();
        }


    }
}

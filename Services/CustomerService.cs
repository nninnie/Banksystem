using BankApp.DTOs;
using BankApp.Models;
using BankApp.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BankApp.Services
{
    // Service class – handles all customer-related data logic
    public class CustomerService : ICustomerService
    {
        private readonly BankAppContext _context;

        public CustomerService(BankAppContext context)
        {
            _context = context;
        }

        // Fetches a customer by ID including dispositions and accounts
        public async Task<Customer?> GetCustomerAsync(int id)
        {
            return await _context.Customers
                .Include(c => c.Dispositions)
                .ThenInclude(d => d.Account)
                .FirstOrDefaultAsync(c => c.CustomerId == id);
        }


        // Calculates the customer's total balance
        public Task<decimal> GetTotalBalanceAsync(Customer customer)
        {
            var total = customer.Dispositions
                .Where(d => d.Account != null)
                .Sum(d => d.Account.Balance);

            return Task.FromResult(total);
        }


        // Returns all accounts belonging to the customer
        public Task<List<Account>> GetAccountsAsync(Customer customer)
        {
            var accounts = customer.Dispositions
                .Where(d => d.Account != null)
                .Select(d => d.Account)
                .ToList();

            return Task.FromResult(accounts);
        }

        // Search + sorting + pagination (50 per page)
        public async Task<List<Customer>> SearchCustomersAsync(
            string? searchName, string? searchCity,
            string? sortColumn, string? sortOrder, int page)
        {
            var query = _context.Customers.AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(searchName))
            {
                query = query.Where(c => c.Givenname.Contains(searchName) ||
                                         c.Surname.Contains(searchName));
            }

            if (!string.IsNullOrWhiteSpace(searchCity))
            {
                query = query.Where(c => c.City.Contains(searchCity));
            }

            // Sort (defaults)
            if (string.IsNullOrEmpty(sortOrder))
                sortOrder = "asc";
            if (string.IsNullOrEmpty(sortColumn))
                sortColumn = "Id";

            if (sortColumn == "Id")
            {
                if (sortOrder == "desc")
                    query = query.OrderByDescending(c => c.CustomerId);
                else
                    query = query.OrderBy(c => c.CustomerId);
            }
            if (sortColumn == "NationalId")
            {
                if (sortOrder == "desc")
                    query = query.OrderByDescending(c => c.NationalId);
                else
                    query = query.OrderBy(c => c.NationalId);
            }
            if (sortColumn == "Name")
            {
                if (sortOrder == "desc")
                    query = query.OrderByDescending(c =>
                        EF.Functions.Collate(c.Surname, "Finnish_Swedish_CI_AS"));
                else
                    query = query.OrderBy(c =>
                        EF.Functions.Collate(c.Surname, "Finnish_Swedish_CI_AS"));
            }

            if (sortColumn == "Address")
            {
                if (sortOrder == "desc")
                    query = query.OrderByDescending(c =>
                        EF.Functions.Collate(c.Streetaddress, "Finnish_Swedish_CI_AS"));
                else
                    query = query.OrderBy(c =>
                        EF.Functions.Collate(c.Streetaddress, "Finnish_Swedish_CI_AS"));
            }

            if (sortColumn == "City")
            {
                if (sortOrder == "desc")
                    query = query.OrderByDescending(c =>
                        EF.Functions.Collate(c.City, "Finnish_Swedish_CI_AS"));
                else
                    query = query.OrderBy(c =>
                        EF.Functions.Collate(c.City, "Finnish_Swedish_CI_AS"));
            }

            // Pagination (afetr sorting)
            if (page < 1) page = 1;

            var firsPageIndex = (page - 1) * 50;

            query = query.Skip(firsPageIndex);
            query = query.Take(50);

            return await query.ToListAsync();
        }

        public async Task<List<CustomerViewModel>> GetTopCustomersByCountryAsync(string country, int count)
        {
            return await _context.Customers
        .Where(c => c.Country == country)
        .Select(c => new CustomerViewModel
        {
            CustomerId = c.CustomerId,
            FullName = c.Givenname + " " + c.Surname,
            City = c.City,
            Country = c.Country,
            TotalBalance = c.Dispositions.Sum(d => d.Account.Balance)
        })
        .OrderByDescending(x => x.TotalBalance)
        .Take(count)
        .ToListAsync();

        }

    }
}

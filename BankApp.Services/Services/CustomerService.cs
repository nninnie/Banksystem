using BankApp.DataAccessLayer.DTOs;
using BankApp.DataAccessLayer.Models;
using BankApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BankApp.Services.Services
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

        // Today's date in Swedish time (Azure servers run on UTC)
        private static DateOnly Today() =>
            DateOnly.FromDateTime(
                TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Europe/Stockholm"));

        // Creates a customer + an owner account in ONE SaveChanges, returns the new customer number
        public async Task<int> CreateCustomerAsync(Customer customer)
        {
            var account = new Account
            {
                Frequency = "Monthly",
                Created = Today(),
                Balance = 0
            };

            customer.Dispositions.Add(new Disposition
            {
                Type = "OWNER",
                Account = account
            });

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return customer.CustomerId;
        }

        // Updates an existing customer, returns false if the customer does not exist
        public async Task<bool> UpdateCustomerAsync(Customer customer)
        {
            var existing = await _context.Customers.FindAsync(customer.CustomerId);
            if (existing == null)
                return false;

            existing.Gender = customer.Gender;
            existing.Givenname = customer.Givenname;
            existing.Surname = customer.Surname;
            existing.Streetaddress = customer.Streetaddress;
            existing.City = customer.City;
            existing.Zipcode = customer.Zipcode;
            existing.Country = customer.Country;
            existing.CountryCode = customer.CountryCode;
            existing.Birthday = customer.Birthday;
            existing.NationalId = customer.NationalId;
            existing.Telephonecountrycode = customer.Telephonecountrycode;
            existing.Telephonenumber = customer.Telephonenumber;
            existing.Emailaddress = customer.Emailaddress;

            await _context.SaveChangesAsync();
            return true;
        }

        // Calculates the customer's total balance
        public decimal GetTotalBalance(Customer customer)
        {
            return customer.Dispositions
                .Where(d => d.Account != null)
                .Sum(d => d.Account.Balance);
        }

        // Returns all accounts belonging to the customer
        public List<Account> GetAccounts(Customer customer)
        {
            return customer.Dispositions
                .Where(d => d.Account != null)
                .Select(d => d.Account)
                .ToList();
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
                query = query.Where(c =>
                c.CustomerId.ToString().Contains(searchName) ||
                (c.Givenname + " " + c.Surname).Contains(searchName));
            }

            if (!string.IsNullOrWhiteSpace(searchCity))
            {
                query = query.Where(c => c.City.Contains(searchCity));
            }

            // Sort
            var desc = sortOrder == "desc";
            const string collation = "Finnish_Swedish_CI_AS";

            // ThenBy CustomerId = tie-breaker, so customers with the same value
            // always come in the same order (needed for correct paging)
            if (sortColumn == "NationalId")
            {
                if (desc)
                    query = query.OrderByDescending(c => c.NationalId).ThenByDescending(c => c.CustomerId);
                else
                    query = query.OrderBy(c => c.NationalId).ThenBy(c => c.CustomerId);
            }
            else if (sortColumn == "Name")
            {
                if (desc)
                    query = query.OrderByDescending(c => EF.Functions.Collate(c.Surname, collation)).ThenByDescending(c => c.CustomerId);
                else
                    query = query.OrderBy(c => EF.Functions.Collate(c.Surname, collation)).ThenBy(c => c.CustomerId);
            }
            else if (sortColumn == "Address")
            {
                if (desc)
                    query = query.OrderByDescending(c => EF.Functions.Collate(c.Streetaddress, collation)).ThenByDescending(c => c.CustomerId);
                else
                    query = query.OrderBy(c => EF.Functions.Collate(c.Streetaddress, collation)).ThenBy(c => c.CustomerId);
            }
            else if (sortColumn == "City")
            {
                if (desc)
                    query = query.OrderByDescending(c => EF.Functions.Collate(c.City, collation)).ThenByDescending(c => c.CustomerId);
                else
                    query = query.OrderBy(c => EF.Functions.Collate(c.City, collation)).ThenBy(c => c.CustomerId);
            }
            else
            {
                // Default: sort by customer number (already unique)
                if (desc)
                    query = query.OrderByDescending(c => c.CustomerId);
                else
                    query = query.OrderBy(c => c.CustomerId);
            }

            // Pagination (after sorting)
            if (page < 1) page = 1;

            var firstPageIndex = (page - 1) * 50;

            query = query.Skip(firstPageIndex);
            query = query.Take(50);

            return await query.ToListAsync();
        }

        // Returns the customers with the highest total balance in a country, limited to the given count
        public async Task<List<CustomerDto>> GetTopCustomersByCountryAsync(string country, int count)
        {
            return await _context.Customers
                .Where(c => c.Country == country)
                .Select(c => new CustomerDto
                {
                    CustomerId = c.CustomerId,
                    FullName = c.Givenname + " " + c.Surname,
                    City = c.City,
                    Country = c.Country,
                    TotalBalance = c.Dispositions
                    .Where(d => d.Type == "OWNER")
                    .Sum(d => d.Account.Balance)
                })
                .OrderByDescending(x => x.TotalBalance)
                .Take(count)
                .ToListAsync();
        }

    }
}

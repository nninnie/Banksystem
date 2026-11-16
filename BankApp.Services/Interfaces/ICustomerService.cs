using BankApp.DataAccessLayer.DTOs;
using BankApp.DataAccessLayer.Models;

namespace BankApp.Services.Interfaces
{
    // Contract for everything the pages need to know about customers (lookup, search, balances, rankings).
    public interface ICustomerService
    {
        Task<Customer?> GetCustomerAsync(int id);

        decimal GetTotalBalance(Customer customer);

        List<Account> GetAccounts(Customer customer);

        Task<List<Customer>> SearchCustomersAsync(
            string? searchName,
            string? searchCity,
            string? sortColumn,
            string? sortOrder,
            int page);
        Task<List<CustomerDto>> GetTopCustomersByCountryAsync(string country, int count);
    }
}

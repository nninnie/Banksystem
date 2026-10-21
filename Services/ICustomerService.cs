using BankApp.Models;
using BankApp.ViewModels;

namespace BankApp.Services
{
    public interface ICustomerService
    {

        Task<Customer?> GetCustomerAsync(int id);

        Task<decimal> GetTotalBalanceAsync(Customer customer);

        Task<List<Account>> GetAccountsAsync(Customer customer);

        Task<List<Customer>> SearchCustomersAsync(
            string? searchName,
            string? searchCity,
            string? sortColumn,
            string? sortOrder,
            int page);
        Task<List<CustomerViewModel>> GetTopCustomersByCountryAsync(string country, int v);
    }
}

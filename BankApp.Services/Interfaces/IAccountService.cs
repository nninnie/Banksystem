using BankApp.DataAccessLayer.DTOs;

namespace BankApp.Services.Interfaces
{
    // Contract for reading account data and its transactions
    public interface IAccountService
    {
        Task<AccountDto?> GetAccountAsync(int accountId);
        Task<List<TransactionDto>> GetTransactionsAsync(int accountId, int skip, int take);
    }
}

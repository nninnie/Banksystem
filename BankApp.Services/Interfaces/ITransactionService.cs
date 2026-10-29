using BankApp.DataAccessLayer.DTOs;

namespace BankApp.Services.Interfaces
{
    // Contract for fetching transaction data, so pages depend on this interface instead of the database.
    public interface ITransactionService
    {
        Task<List<TransactionDto>> GetRecentTransactionsAsync(int count);
    }
}
using BankApp.ViewModels;

namespace BankApp.Services
{
    // Contract for fetching transaction data, so pages depend on this interface instead of the database.
    public interface ITransactionService
    {
        Task<List<TransactionViewModel>> GetRecentTransactionsAsync(int count);
    }
}
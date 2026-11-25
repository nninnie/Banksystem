using BankApp.DataAccessLayer.DTOs;
using BankApp.Services.Enums;

namespace BankApp.Services.Interfaces
{
    // Contract for reading account data and its transactions
    public interface IAccountService
    {
        Task<AccountDto?> GetAccountAsync(int accountId);
        Task<List<TransactionDto>> GetTransactionsAsync(int accountId, int skip, int take);
        Task<int> GetTransactionCountAsync(int accountId);
        Task<ErrorCode> DepositAsync(int accountId, decimal amount);
        Task<ErrorCode> WithdrawAsync(int accountId, decimal amount);
        Task<ErrorCode> TransferAsync(int fromAccountId, int toAccountId, decimal amount);
        bool IsLargeWithdrawal(decimal amount);
    }
}

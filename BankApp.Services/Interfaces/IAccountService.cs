using BankApp.DataAccessLayer.DTOs;
using BankApp.Services.Enums;

namespace BankApp.Services.Interfaces
{
    // Contract for reading account data and its transactions
    public interface IAccountService
    {
        Task<AccountDto?> GetAccountAsync(int accountId);
        Task<List<TransactionDto>> GetTransactionsAsync(int accountId, int skip, int take);

        // Deposit+ Withdraw + Transfer – returns OK or the reason it failed
        Task<ErrorCode> DepositAsync(int accountId, decimal amount);
        Task<ErrorCode> WithdrawAsync(int accountId, decimal amount);
        Task<ErrorCode> TransferAsync(int fromAccountId, int toAccountId, decimal amount);
    }
}

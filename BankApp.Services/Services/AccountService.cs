using BankApp.DataAccessLayer.DTOs;
using BankApp.DataAccessLayer.Models;
using BankApp.Services.Enums;
using BankApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace BankApp.Services.Services
{
    // Service class – handles account data and transaction history
    public class AccountService : IAccountService
    {
        private readonly BankAppContext _context;

        public AccountService(BankAppContext context)
        {
            _context = context;
        }

        // Today's date in Swedish time (Azure servers run on UTC)
        private static DateOnly Today() =>
            DateOnly.FromDateTime(
                TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Europe/Stockholm"));

        // Fetches a single account (number + balance)
        public async Task<AccountDto?> GetAccountAsync(int accountId)
        {
            return await _context.Accounts
                .AsNoTracking()
                .Where(a => a.AccountId == accountId)
                .Select(a => new AccountDto
                {
                    AccountId = a.AccountId,
                    Frequency = a.Frequency,
                    Created = a.Created.ToDateTime(TimeOnly.MinValue),
                    Balance = a.Balance,

                    // Owner via Dispositions
                    CustomerId = a.Dispositions
                        .Where(d => d.Type == "OWNER")
                        .Select(d => d.CustomerId)
                        .FirstOrDefault(),
                    CustomerName = a.Dispositions
                        .Where(d => d.Type == "OWNER")
                        .Select(d => d.Customer.Givenname + " " + d.Customer.Surname)
                        .FirstOrDefault() ?? "",
                    Country = a.Dispositions
                        .Where(d => d.Type == "OWNER")
                        .Select(d => d.Customer.Country)
                        .FirstOrDefault() ?? ""
                })
                .FirstOrDefaultAsync();
        }

        // Total number of transactions on the account, for "Showing 40 of N"
        public async Task<int> GetTransactionCountAsync(int accountId)
        {
            return await _context.Transactions
                .CountAsync(t => t.AccountId == accountId);
        }

        // Fetches transactions newest first, in chunks (used for AJAX "load more")
        public async Task<List<TransactionDto>> GetTransactionsAsync(int accountId, int skip, int take)
        {
            var rows = await _context.Transactions
                .AsNoTracking()
                .Where(t => t.AccountId == accountId)
                .OrderByDescending(t => t.Date)
                .ThenByDescending(t => t.TransactionId)
                .Skip(skip) 
                .Take(take) 
                .ToListAsync();

            return rows.Select(t => new TransactionDto
            {
                TransactionId = t.TransactionId,
                AccountId = t.AccountId,
                Date = t.Date.ToDateTime(TimeOnly.MinValue),
                Type = t.Type,
                Operation = t.Operation,
                Amount = t.Amount,
                Balance = t.Balance
            }).ToList();
        }

        // Deposit – adds money to the account and saves a Credit transaction
        public async Task<ErrorCode> DepositAsync(int accountId, decimal amount)
        {
            if (amount <= 0)
                return ErrorCode.IncorrectAmount;

            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountId == accountId);

            if (account == null)
                return ErrorCode.AccountNotFound;

            account.Balance += amount;

            _context.Transactions.Add(new Transaction
            {
                AccountId = accountId,
                Date = Today(),
                Type = "Credit",
                Operation = "Credit in Cash",
                Amount = amount,
                Balance = account.Balance
            });

            await _context.SaveChangesAsync();
            return ErrorCode.OK;

        }

        // Withdraw – removes money from the account and saves a Debit transaction
        public async Task<ErrorCode> WithdrawAsync(int accountId, decimal amount)
        {
            if (amount <= 0)
                return ErrorCode.IncorrectAmount;

            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountId == accountId);

            if (account == null)
                return ErrorCode.AccountNotFound;

            if (amount > account.Balance)
                return ErrorCode.BalanceTooLow;

            account.Balance -= amount;

            _context.Transactions.Add(new Transaction
            {
                AccountId = accountId,
                Date = Today(),
                Type = "Debit",
                Operation = "Withdrawal in Cash",
                Amount = -amount,
                Balance = account.Balance
            });

            await _context.SaveChangesAsync();
            return ErrorCode.OK;
        }


        // Transfer – moves money between two accounts, one transaction on each
        public async Task<ErrorCode> TransferAsync(int fromAccountId, int toAccountId, decimal amount)
        {
            if (amount <= 0)
                return ErrorCode.IncorrectAmount;

            if (fromAccountId == toAccountId)
                return ErrorCode.SameAccount;

            var fromAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountId == fromAccountId);
            var toAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountId == toAccountId);

            if (fromAccount == null || toAccount == null)
                return ErrorCode.AccountNotFound;

            if (amount > fromAccount.Balance)
                return ErrorCode.BalanceTooLow;

            fromAccount.Balance -= amount;
            toAccount.Balance += amount;

            var today = Today();

            _context.Transactions.Add(new Transaction
            {
                AccountId = fromAccountId,
                Date = today,
                Type = "Debit",
                Operation = $"Transfer to account {toAccountId}",
                Amount = -amount,
                Balance = fromAccount.Balance
            });

            _context.Transactions.Add(new Transaction
            {
                AccountId = toAccountId,
                Date = today,
                Type = "Credit",
                Operation = $"Transfer from account {fromAccountId}",
                Amount = amount,
                Balance = toAccount.Balance
            });

            await _context.SaveChangesAsync();
            return ErrorCode.OK;
        }
    }
}

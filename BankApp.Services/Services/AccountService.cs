using BankApp.DataAccessLayer.DTOs;
using BankApp.DataAccessLayer.Models;
using BankApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using BankApp.Services.Enums;


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
                        .FirstOrDefault() ?? ""
                })
                .FirstOrDefaultAsync();
        }

        // Fetches transactions newest first, in chunks (used for AJAX "load more")
        public async Task<List<TransactionDto>> GetTransactionsAsync(int accountId, int skip, int take)
        {
            var rows = await _context.Transactions
                .AsNoTracking()
                .Where(t => t.AccountId == accountId)
                .OrderByDescending(t => t.Date)
                .ThenByDescending(t => t.TransactionId)
                .Skip(skip) // Needed for AJAX
                .Take(take) // Needed for AJAX
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

            // The balance only changes together with a transaction row
            _context.Transactions.Add(new Transaction
            {
                AccountId = accountId,
                Date = DateOnly.FromDateTime(DateTime.Today),
                Type = "Credit",
                Operation = "Credit in Cash",
                Amount = amount,
                Balance = account.Balance
            });

            await _context.SaveChangesAsync();
            return ErrorCode.OK;

        }
    }
}

using BankApp.DataAccessLayer.DTOs;

namespace BankApp.Services.Interfaces
{
    // Contract for finding suspicious transactions (money laundering rules)
    public interface ISuspiciousTransactionService
    {
        Task<List<string>> GetCountriesAsync();
        Task<List<SuspiciousTransactionDto>> FindSuspiciousAsync(string country, int afterTransactionId, DateOnly? since = null);

        Task<int> GetLastTransactionIdAsync();
    }
}
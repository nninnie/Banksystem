namespace BankApp.DTOs
{
    // Account DTO – provides only essential, non-sensitive account data to the UI.
    // Excludes EF navigation properties and internal banking fields to ensure data safety.
    public class AccountDto
    {
        public int AccountId { get; set; }
        public string Frequency { get; set; }
        public DateTime Created { get; set; }   // DateOnly → DateTime for UI-support
        public decimal Balance { get; set; }
    }
}

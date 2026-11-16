namespace BankApp.DataAccessLayer.DTOs
{
    // Transaction DTO – exposes only essential transaction data to the UI.
    // Excludes EF navigation properties and internal banking fields such as Symbol, Bank, and Account.
    public class TransactionDto
    {
        public int TransactionId { get; set; }
        public int AccountId { get; set; }
        public string CustomerName { get; set; } = "";
        public DateTime Date { get; set; }   // DateOnly → DateTime for UI-support
        public string Type { get; set; }
        public string? Operation { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
    }
}

namespace BankApp.DataAccessLayer.DTOs
{
    // DTO - Represents one suspicious transaction found by the console app
    public class SuspiciousTransactionDto
    {
        public string Country { get; set; } = "";
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = "";
        public int AccountId { get; set; }
        public int TransactionId { get; set; }
        public DateOnly Date { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; } = "";
    }
}
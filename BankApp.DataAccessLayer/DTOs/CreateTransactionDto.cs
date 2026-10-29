namespace BankApp.DataAccessLayer.DTOs
{
    // Transaction creation DTO – ensures the UI provides only essential data.
    // Avoids accidental submission of full Account entities from the UI.
    public class CreateTransactionDto
    {
        public int AccountId { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; } // Transactiontype (Deposit / Withdrawal / Payment / Transfer)
    }
}

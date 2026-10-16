namespace BankApp.DTOs
{
    // DTO för att skapa transaktioner – UI skickar bara det som behövs.
    // Hindrar UI från att skicka hela Account-objektet av misstag.
    public class CreateTransactionDto
    {
        public int AccountId { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; } // Transaktionstyp (Deposit / Withdrawal / Payment / Transfer)
    }
}

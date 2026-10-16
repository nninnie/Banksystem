namespace BankApp.DTOs
{
    // DTO för Transaction – UI får bara ren transaktionsinfo.
    // Tar bort EF‑navigationer och interna bankfält (Symbol, Bank, Account).
    public class TransactionDto
    {
        public int TransactionId { get; set; }
        public DateTime Date { get; set; }   // DateOnly → DateTime för UI-stöd
        public string Type { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
    }
}

namespace BankApp.DTOs
{
    // DTO för Account – skickar bara säker och relevant kontoinfo till UI.
    // Hindrar UI från att få EF‑relationer, interna fält och känslig data.
    public class AccountDto
    {
        public int AccountId { get; set; }
        public string Frequency { get; set; }
        public DateTime Created { get; set; }   // DateOnly → DateTime för UI-stöd
        public decimal Balance { get; set; }
    }
}

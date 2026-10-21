namespace BankApp.ViewModels
{
    // ViewModel - Represents a transaction in the bank database
    public class TransactionViewModel
    {
        public int TransactionId { get; set; }
        public int AccountId { get; set; }
        public string CustomerName { get; set; } = "";
        public string Type { get; set; } = "";       // Credit / Debit
        public string Operation { get; set; } = "";  // Withdrawal, Transfer
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }         
        public DateTime Date { get; set; }

        // Helper property to prevent the view from having to compare strings itself
        public bool IsDebit => Type == "Debit";
    }
}

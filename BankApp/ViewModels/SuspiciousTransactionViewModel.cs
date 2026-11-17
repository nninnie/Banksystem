namespace BankApp.ViewModels
{
    // ViewModel - One suspicious transaction shown in the Admin list
    public class SuspiciousTransactionViewModel
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = null!;
        public int AccountId { get; set; }
        public int TransactionId { get; set; }
        public DateOnly Date { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; } = null!;
    }
}
using System.ComponentModel.DataAnnotations;

namespace BankApp.ViewModels
{
    // ViewModel - Represents a transaction in the bank database, used when receiving data from a form (Input/Post Model)
    public class TransactionViewModel
    {
        public int TransactionId { get; set; }

        [Required(ErrorMessage = "Account ID is required.")]
        public int AccountId { get; set; }

        public string CustomerName { get; set; } = "";

        [Required(ErrorMessage = "Transaction type is required.")]
        [RegularExpression("^(Credit|Debit)$", ErrorMessage = "Type must be either 'Credit' or 'Debit'.")]
        public string Type { get; set; } = "";

        [Required(ErrorMessage = "Operation type is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Operation must be between 3 and 50 characters.")]
        public string Operation { get; set; } = "";  // e.g., Withdrawal, Deposit, Transfer

        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, 10000000, ErrorMessage = "Amount must be greater than 0.")]
        public decimal Amount { get; set; }

        public decimal Balance { get; set; }

        public DateTime Date { get; set; }

        public bool IsDebit => Type == "Debit";
    }
}

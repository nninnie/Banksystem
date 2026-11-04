using System.ComponentModel.DataAnnotations;

namespace BankApp.ViewModels
{
    // ViewModel Input for a transfer – what the cashier types in the form
    public class TransferViewModel
    {
        [Required(ErrorMessage = "Receiving account is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Enter a valid account number.")]
        public int? ToAccountId { get; set; }

        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, 1000000, ErrorMessage = "Amount must be between 0.01 and 1 000 000.")]
        public decimal Amount { get; set; }
    }
}
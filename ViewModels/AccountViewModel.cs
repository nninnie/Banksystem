using System.ComponentModel.DataAnnotations;

namespace BankApp.ViewModels
{

    // ViewModel – represents a customer account
    public class AccountViewModel
    {
        public int AccountId { get; set; }

        [Required(ErrorMessage = "Initial balance is required.")]
        [Range(0, 100000000, ErrorMessage = "Balance cannot be negative.")]
        public decimal Balance { get; set; }

        [Required(ErrorMessage = "Account frequency is required.")]
        [StringLength(50, ErrorMessage = "Frequency cannot exceed 50 characters.")]
        public string Frequency { get; set; } = string.Empty;
    }
}
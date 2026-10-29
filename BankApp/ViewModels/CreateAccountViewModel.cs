using System.ComponentModel.DataAnnotations;

namespace BankApp.ViewModels
{
    // ViewModel used to create a customer account, includes validation because it handles user input.
    public class CreateAccountViewModel
    {
        [Required(ErrorMessage = "Customer is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid customer.")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Account frequency is required.")]
        [RegularExpression("^(Monthly|Weekly|AfterTransaction)$",
    ErrorMessage = "Frequency must be Monthly, Weekly or AfterTransaction.")]
        public string Frequency { get; set; } = "Monthly";

        [Range(0, 100000000, ErrorMessage = "Initial deposit must be between 0 and 100,000,000.")]
        [DataType(DataType.Currency)]
        public decimal InitialDeposit { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace BankApp.ViewModels
{
    // ViewModel – the amount entered on the Deposit and Withdraw forms
    public class DepositWithdrawViewModel
    {
        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, 1000000, ErrorMessage = "Amount must be between 0.01 and 1 000 000.")]
        public decimal Amount { get; set; }
    }
}
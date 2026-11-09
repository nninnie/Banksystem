using System.ComponentModel.DataAnnotations;

namespace BankApp.ViewModels
{
    // ViewModel - the customer number typed in the quick search (navbar and Find page)
    public class NavbarSearchViewModel
    {
        [Required(ErrorMessage = "Enter a customer number.")]
        [Range(1, int.MaxValue, ErrorMessage = "Enter a valid customer number.")]
        public int? CustomerId { get; set; }
    }
}
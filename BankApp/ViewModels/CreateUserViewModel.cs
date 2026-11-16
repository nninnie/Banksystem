using System.ComponentModel.DataAnnotations;

namespace BankApp.ViewModels
{
    // ViewModel - Represents the form for a new system user (Input/Post Model)
    public class CreateUserViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [StringLength(256, ErrorMessage = "Email can be max 256 characters.")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";

        [Required(ErrorMessage = "Please repeat the password.")]
        [Compare("Password", ErrorMessage = "The passwords do not match.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = "";

        [Required(ErrorMessage = "Role is required.")]
        [RegularExpression("^(Admin|Cashier)$", ErrorMessage = "Choose Admin or Cashier.")]
        public string Role { get; set; } = "";
    }
}
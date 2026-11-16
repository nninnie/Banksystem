using System.ComponentModel.DataAnnotations;

namespace BankApp.ViewModels
{
    // ViewModel - Represents the form for editing a system user (Input/Post Model)
    public class EditUserViewModel
    {
        public string Id { get; set; } = "";

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [StringLength(256, ErrorMessage = "Email can be max 256 characters.")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Role is required.")]
        [RegularExpression("^(Admin|Cashier)$", ErrorMessage = "Choose Admin or Cashier.")]
        public string Role { get; set; } = "";

        // Optional - leave empty to keep the current password
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters.")]
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "The passwords do not match.")]
        [DataType(DataType.Password)]
        public string? ConfirmNewPassword { get; set; }
    }
}
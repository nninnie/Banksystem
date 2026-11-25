using System.ComponentModel.DataAnnotations;

namespace BankApp.ViewModels
{
    // ViewModel - Represents a customer in the bank database
    public class CustomerViewModel
    {
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Full name must be between 2 and 100 characters.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Gender selection is required.")]
        [RegularExpression("^(Male|Female|Other)$", ErrorMessage = "Gender must be 'Male', 'Female', or 'Other'.")]
        public string Gender { get; set; } = string.Empty;

        [Required(ErrorMessage = "Street address is required.")]
        [StringLength(150, ErrorMessage = "Street address cannot exceed 150 characters.")]
        public string Streetaddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "City is required.")]
        [StringLength(100, ErrorMessage = "City cannot exceed 100 characters.")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "Zip code is required.")]
        [RegularExpression(@"^[0-9 ]{4,6}$", ErrorMessage = "Zip code must be 4-6 digits (spaces allowed).")]
        [StringLength(15, ErrorMessage = "Zip code cannot exceed 15 characters.")]
        public string Zipcode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Country is required.")]
        [StringLength(100, ErrorMessage = "Country cannot exceed 100 characters.")]
        public string Country { get; set; } = string.Empty;

        [Required(ErrorMessage = "National ID is required.")]
        [StringLength(20, ErrorMessage = "National ID cannot exceed 20 characters.")]
        public string NationalId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telephone number is required.")]
        [Phone(ErrorMessage = "Invalid telephone number format.")]
        public string Telephone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string Email { get; set; } = string.Empty;

        public decimal TotalBalance { get; set; }
        public DateOnly? Birthday { get; set; }
        public List<AccountViewModel> Accounts { get; set; } = new List<AccountViewModel>();
    }
}
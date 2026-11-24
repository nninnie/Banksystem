using System.ComponentModel.DataAnnotations;

namespace BankApp.ViewModels
{
    // ViewModel - Form data for creating and editing a customer
    public class CustomerFormViewModel
    {
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        [RegularExpression("^(male|female)$", ErrorMessage = "Choose male or female.")]
        public string Gender { get; set; } = "";

        [Required(ErrorMessage = "First name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 100 characters.")]
        public string Givenname { get; set; } = "";

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(100, ErrorMessage = "Last name can be max 100 characters.")]
        public string Surname { get; set; } = "";

        [Required(ErrorMessage = "Street address is required.")]
        [StringLength(100, ErrorMessage = "Street address can be max 100 characters.")]
        public string Streetaddress { get; set; } = "";

        [Required(ErrorMessage = "City is required.")]
        [StringLength(100, ErrorMessage = "City can be max 100 characters.")]
        public string City { get; set; } = "";

        [Required(ErrorMessage = "Zip code is required.")]
        [RegularExpression(@"^[0-9 ]{4,6}$", ErrorMessage = "Zip code must be 4-6 digits (spaces allowed).")]
        [StringLength(15, ErrorMessage = "Zip code can be max 15 characters.")]
        public string Zipcode { get; set; } = "";

        [Required(ErrorMessage = "Country is required.")]
        [RegularExpression("^(Sweden|Norway|Denmark|Finland)$", ErrorMessage = "Choose a country from the list.")]
        public string Country { get; set; } = "";

        [Required(ErrorMessage = "Birthday is required.")]
        public DateOnly? Birthday { get; set; }

        [Required(ErrorMessage = "National ID is required.")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "National ID must be between 6 and 20 characters.")]
        public string NationalId { get; set; } = "";

        [RegularExpression(@"^\+?\d{1,4}$", ErrorMessage = "Country code must look like +46.")]
        [StringLength(10, ErrorMessage = "Country code can be max 10 characters.")]
        public string? Telephonecountrycode { get; set; }

        [RegularExpression(@"^[0-9 -]{5,25}$", ErrorMessage = "Phone number can only have digits, space and dash (5-25 characters).")]
        public string? Telephonenumber { get; set; }

        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [StringLength(100, ErrorMessage = "Email can be max 100 characters.")]
        public string? Emailaddress { get; set; }
    }
}
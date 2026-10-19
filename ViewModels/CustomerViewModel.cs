namespace BankApp.ViewModels
{
    // ViewModel - Represents a customer in the bank database
    public class CustomerViewModel
    {
        public int CustomerId { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public string Streetaddress { get; set; }
        public string City { get; set; }
        public string Zipcode { get; set; }
        public string Country { get; set; }
        public string NationalId { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }

        public decimal TotalBalance { get; set; }
        public List<AccountViewModel> Accounts { get; set; }
    }

}

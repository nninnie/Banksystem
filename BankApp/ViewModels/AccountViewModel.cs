
namespace BankApp.ViewModels
{

    // ViewModel – represents a customer account, no validation
    public class AccountViewModel
    {
        public int AccountId { get; set; }
        public string Frequency { get; set; } = string.Empty;
        public DateTime Created { get; set; }
        public decimal Balance { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
    }
}
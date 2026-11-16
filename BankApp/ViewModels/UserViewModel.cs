namespace BankApp.ViewModels
{
    // ViewModel - Represents a system user in the list, no validation
    public class UserViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsProtected { get; set; }
    }
}
namespace BankApp.DataAccessLayer.DTOs
{
    // User DTO – a system user (Admin or Cashier) with the role as text
    public class UserDto
    {
        public string Id { get; set; } = "";
        public string Email { get; set; } = "";
        public string Role { get; set; } = "";
        public bool IsProtected { get; set; }
    }
}
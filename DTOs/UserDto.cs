namespace BankApp.DTOs
{
    // User DTO – excludes PasswordHash and internal Identity fields.
    // Only exposes safe, non-sensitive data to the UI.
    public class UserDto
    {
        public int UserId { get; set; }
        public string LoginName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}

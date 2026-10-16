namespace BankApp.DTOs
{
    // DTO för User – UI får ALDRIG PasswordHash eller interna Identity‑fält.
    // Visar bara det som är säkert att visa i UI.
    public class UserDto
    {
        public int UserId { get; set; }
        public string LoginName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}

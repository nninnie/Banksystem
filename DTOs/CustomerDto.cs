namespace BankApp.DTOs
{
    // Customer DTO – exposes only non-sensitive fields such as name and location.
    // Sensitive customer information (address, personal ID number, phone, email) is intentionally excluded.
    public class CustomerDto
    {
        public int CustomerId { get; set; }
        public string FullName { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
    }
}

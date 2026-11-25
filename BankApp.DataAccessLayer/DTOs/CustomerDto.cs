namespace BankApp.DataAccessLayer.DTOs
{
    // Customer DTO – exposes only non-sensitive fields such as name and location
    public class CustomerDto
    {
        public int CustomerId { get; set; }
        public string FullName { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public decimal TotalBalance { get; set; }
    }
}

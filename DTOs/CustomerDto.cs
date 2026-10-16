namespace BankApp.DTOs
{
    // DTO för Customer – UI får bara namn + plats.
    // Skyddar känsliga kunduppgifter (adress, personnummer, telefon, email).
    public class CustomerDto
    {
        public int CustomerId { get; set; }
        public string FullName { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
    }
}

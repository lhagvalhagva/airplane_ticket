namespace RestApi.DTOs
{
    public class PassengerDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PassportNumber { get; set; }
        public string Nationality { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public bool CheckedIn { get; set; }
    }
} 
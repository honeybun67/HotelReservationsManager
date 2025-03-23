namespace HotelReservationsManager.Data.Models
{
    public class Client
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Number { get; set; }

        public string? Email { get; set; }

        public bool IsAdult { get; set; }
    }
}

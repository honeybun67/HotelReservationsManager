namespace HotelReservationsManager.Data.Models
{
    public class Reservation
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string RoomId { get; set; }
        public string UserId { get; set; }
        public ICollection<Client> Clients { get; set; } = new HashSet<Client>();
        public DateOnly AccommodationDate { get; set; }
        public DateOnly EmptyDate { get; set; }
        public bool WithBreakfast { get; set; }
        public bool AllInclusive { get; set; }

    }
}

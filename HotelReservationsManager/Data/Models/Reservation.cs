using System.ComponentModel.DataAnnotations.Schema;

namespace HotelReservationsManager.Data.Models
{
    public class Reservation
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string RoomId { get; set; }
        public virtual Room Room { get; set; }
        public string UserId { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<Client> Clients { get; set; } = new List<Client>();
        public DateTime AccommodationDate { get; set; }
        public DateTime LeaveDate { get; set; }
        public bool HasBreakfast { get; set; }
        public bool HasAllInclusive { get; set; }
        [Column(TypeName = "money")]
        public double Price { get; set; }
    }
}

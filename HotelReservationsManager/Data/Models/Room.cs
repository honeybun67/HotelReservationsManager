using HotelReservationsManager.Data.Models.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelReservationsManager.Data.Models
{
    public class Room
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public int Capacity { get; set; }
        public int Number { get; set; }
        public RoomType Type { get; set; }
        public bool IsAvailable { get; set; }
        public double PricePerAdultBed { get; set; }
        public double PricePerChildBed { get; set; }
         public string? ReservationId { get; set; }
        [ForeignKey("ReservationId")]
         public virtual Reservation Reservation { get; set; }
    }
}

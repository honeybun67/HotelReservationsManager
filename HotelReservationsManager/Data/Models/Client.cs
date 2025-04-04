﻿namespace HotelReservationsManager.Data.Models
{
    public class Client
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Number { get; set; }

        public string? Email { get; set; }

        public bool IsAdult { get; set; }
        public string? ReservationId { get; set; }
        public virtual Reservation Reservation { get; set; }

        public virtual ICollection<ClientHistory> ClientHistories { get; set; } = new HashSet<ClientHistory>();
    }
}

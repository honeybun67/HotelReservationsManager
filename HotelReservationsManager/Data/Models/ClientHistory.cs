using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HotelReservationsManager.Data.Models
{
    public class ClientHistory
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string ClientId { get; set; }
        public virtual Client Client { get; set; }
        public int ResRoomNumber { get; set; }
        public DateTime AccomodationDate { get; set; }
        public DateTime LeaveDate { get; set; }

        [Column(TypeName = "money")]
        [DataType(DataType.Currency)]
        public double ResPrice { get; set; }

        public virtual ICollection<ClientHistory> ClientHistories { get; set; } = new List<ClientHistory>();
    }
}

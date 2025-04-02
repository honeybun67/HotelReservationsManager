using HotelReservationsManager.Data.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HotelReservationsManager.ViewModels.Reservations
{
    public class CreateReservationViewModel
    {
        public string UserId { get; set; }
        public int RoomCapacity { get; set; }

        [Required(ErrorMessage = "Please select and submit a room")]
        public string RoomId { get; set; }
        public SelectList? Rooms { get; set; }
        public virtual ICollection<Client> Clients { get; set; } = new HashSet<Client>();

        [Required]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [DisplayName("Accomodation date")]
        public DateTime AccommodationDate { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [DisplayName("Accomodation date")]
        public DateTime LeaveDate { get; set; }
        [DisplayName("Breakfast")]
        public bool WithBreakfast { get; set; }
        [DisplayName("Allinclusive")]
        public bool AllInclusive { get; set; }
    }
}

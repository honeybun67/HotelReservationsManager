using System.ComponentModel.DataAnnotations;

namespace HotelReservationsManager.ViewModels.Clients
{
    public class CreateClientViewModel
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string Email { get; set; }
        public bool IsAdult { get; set; }
    }
}

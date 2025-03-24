using System.ComponentModel.DataAnnotations;

namespace HotelReservationsManager.ViewModels.Clients
{
    public class ClientDetailsViewModel
    {
        public string Id { get; set; }
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Display(Name = "Phone number")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Adult")]
        public bool IsAdult { get; set; }
        public ICollection<ClientHistoryViewModel> History { get; set; } = new List<ClientHistoryViewModel>();

    }
}

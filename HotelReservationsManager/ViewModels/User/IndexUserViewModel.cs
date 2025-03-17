using System.ComponentModel.DataAnnotations;

namespace HotelReservationsManager.ViewModels.User
{
    public class IndexUserViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string UCN { get; set; }
        [DisplayFormat(DataFormatString = "{dd/MM/yyyy}")]
        public DateTime HireDate { get; set; }
        public bool Status { get; set; }
        [DisplayFormat(DataFormatString = "{dd/MM/yyyy}")]
        public DateTime? QuitDate { get; set; }
        public string Role { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace HotelReservationsManager.ViewModels.User
{
    public class EditUserViewModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool Status { get; set; }
        public bool Role { get; set; }
    }
}

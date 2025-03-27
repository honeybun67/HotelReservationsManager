using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace HotelReservationsManager.Data.Models
{
    public class User:IdentityUser
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string UCN { get; set; }
        public DateTime HireDate { get; set; }
        public bool Status { get; set; }
        public DateTime? QuitDate { get; set; }
    }
}

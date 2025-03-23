using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelReservationsManager.ViewModels.Users
{
    public class IndexUserViewModel
    {
        public string Id { get; set; }

        [Display(Name = "Full name")]
        public string Name { get; set; }

        public string Email { get; set; }

        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        public string UCN { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [DataType(DataType.Date)]
        [Display(Name = "Hire date")]

        public DateTime HireDate { get; set; }

        public bool Status { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [DataType(DataType.Date)]
        [Display(Name = "Quit date")]
        public DateTime? QuitDate { get; set; }


        [Display(Name = "User roles")]
        public string Role { get; set; }
    }
}

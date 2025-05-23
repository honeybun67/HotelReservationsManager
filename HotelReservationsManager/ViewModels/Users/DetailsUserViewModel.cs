﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelReservationsManager.ViewModels.Users
{
    public class DetailsUserViewModel
    {
        public string Id { get; set; }
        [Display(Name = "Full name")]
        public string Name { get; set; }

        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }
        [Display(Name = "UCN")]
        public string UCN { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [DataType(DataType.Date)]
        [Display(Name = "Hire date")]
        public DateTime HireDate { get; set; }
        [Display(Name = "Status")]
        public bool Status { get; set; }
        [Display(Name = "Roles")]
        public string Role { get; set; }
    }
}

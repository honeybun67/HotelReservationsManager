﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelReservationsManager.ViewModels.Users
{
    public class EditUserViewModel
    {
        public string Id { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(100)]

        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(100)]

        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Display(Name = "Status")]
        public bool Status { get; set; }
        [Display(Name = "User role")]
        public bool Role { get; set; }
    }
}

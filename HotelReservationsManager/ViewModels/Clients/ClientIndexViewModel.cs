﻿using System.ComponentModel.DataAnnotations;

namespace HotelReservationsManager.ViewModels.Clients
{
    public class ClientIndexViewModel
    {
        public string Id { get; set; }
        [Display(Name = "First name")]
        public string FirstName { get; set; }
        [Display(Name = "Last name")]
        public string LastName { get; set; }
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Display(Name = "Adult")]
        public bool IsAdult { get; set; }
        public bool RemoveFromRes { get; set; } = false;
    }
}

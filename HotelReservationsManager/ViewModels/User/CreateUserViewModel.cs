using System.ComponentModel.DataAnnotations;

namespace HotelReservationsManager.ViewModels.User
{
    public class CreateUserViewModel
    {
        [Required]

        public string FirstName { get; set; }
        [Required]
        public string MiddleName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(10)]
        public string PhoneNumber { get; set; }
        [Required]
        [StringLength(10)]
        [MinLength(10)]
        public string UCN { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{dd/MM/yyyy}")]
        [DataType(DataType.Date)]
        public DateTime HireDate { get; set; }
        public bool Status { get; set; }

        [DisplayFormat(DataFormatString = "{dd/MM/yyyy}")]
        [DataType(DataType.Date)]
        public DateTime? QuitDate { get; set; }

        [Required]
        [StringLength(100)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}

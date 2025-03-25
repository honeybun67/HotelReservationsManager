using HotelReservationsManager.Data.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace HotelReservationsManager.ViewModels.Rooms
{
    public class IndexRoomViewModel
    {
        public string Id { get; set; }
        public int Number { get; set; }
        [Display(Name = "Capacity")]
        public int Capacity { get; set; }
        [Display(Name = "Available")]
        public bool IsAvailable { get; set; }
        [Display(Name = "Room Type")]
        public RoomType RoomType { get; set; }
        [DataType(DataType.Currency)]
        [Display(Name = "Price for adult")]
        public double PricePerAdultBed { get; set; }
        [DataType(DataType.Currency)]
        [Display(Name = "Price for child")]
        public double PricePerChildBed { get; set; }
    }
}

using HotelReservationsManager.Data.Models.Enum;

namespace HotelReservationsManager.ViewModels.Rooms
{
    public class SelectListRoomViewModel
    {
        public string Id { get; set; }
        public int Capacity { get; set; }
        public int Number { get; set; }
        public RoomType RoomType { get; set; }
        public double PricePerAdultBed { get; set; }
        public double PricePerChildBed { get; set; }
    }
}

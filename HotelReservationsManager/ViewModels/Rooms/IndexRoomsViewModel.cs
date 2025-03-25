namespace HotelReservationsManager.ViewModels.Rooms
{
    public class IndexRoomsViewModel : PagingViewModel
    {
        public IndexRoomsViewModel(int elementsCount, int itemsPerPage = 5, string action = "Index") : base(elementsCount, itemsPerPage, action)
        {
        }
        public IndexRoomsViewModel() : base(0)
        {

        }
        public string FilterByType { get; set; }
        public int Capacity { get; set; }
        public bool IsAvailable { get; set; }
        public ICollection<IndexRoomViewModel> Rooms { get; set; } = new List<IndexRoomViewModel>();
    }
}

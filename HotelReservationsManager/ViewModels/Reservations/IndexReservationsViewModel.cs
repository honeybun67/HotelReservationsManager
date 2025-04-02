namespace HotelReservationsManager.ViewModels.Reservations
{
    public class IndexReservationsViewModel:PagingViewModel
    {
        public IndexReservationsViewModel(int elementsCount, int itemsPerPage = 5, string action = "Index") : base(elementsCount, itemsPerPage, action)
        {
        }

        public IndexReservationsViewModel() : base(0)
        {

        }
        public ICollection<IndexReservationViewModel> Reservations { get; set; } = new List<IndexReservationViewModel>();
    }
}

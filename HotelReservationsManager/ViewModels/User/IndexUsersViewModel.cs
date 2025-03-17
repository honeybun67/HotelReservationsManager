namespace HotelReservationsManager.ViewModels.User
{
    public class IndexUsersViewModel : PagingViewModel
    {
        public IndexUsersViewModel() : base(0)
        {

        }
        public IndexUsersViewModel(int elementsCount, int itemsPerPage = 10, string action = "Index") : base(elementsCount, itemsPerPage, action)
        {
        }
        public string FilterByName { get; set; }
        public ICollection<IndexUserViewModel> Users { get; set; } = new List<IndexUserViewModel>();

    }
}

using System.ComponentModel.DataAnnotations;
using HotelReservationsManager.ViewModels.Clients;
using HotelReservationsManager.ViewModels;

namespace HotelReservationsManager.ViewModels.Clients
{
    public class ClientsIndexViewModel : PagingViewModel
    {
        public ClientsIndexViewModel(int elementsCount, int itemsPerPage = 5, string action = "Index") : base(elementsCount, itemsPerPage, action)
        {
        }
        public ClientsIndexViewModel() : base(0)
        {

        }
        public string FilterByName { get; set; }
        public ICollection<ClientIndexViewModel> Clients { get; set; } = new List<ClientIndexViewModel>();
    }
}

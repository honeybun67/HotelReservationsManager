using HotelReservationsManager.Data.Models;
using HotelReservationsManager.ViewModels.Reservations;
using HotelReservationsManager.ViewModels.Rooms;

namespace HotelReservationsManager.Services.Contracts
{
    public interface IReservationsService
    {
        public Task CreateReservationAsync(CreateReservationViewModel model);
        public Task<IndexReservationsViewModel> GetReservationsAsync(IndexReservationsViewModel model);
        public Task<List<SelectListRoomViewModel>> GetFreeRoomsSelectListAsync();
        public Task<List<SelectListRoomViewModel>> GetAllRoomsSelectListAsync(EditReservationViewModel model);
        public Task<int> GetRoomCapacityAsync(string id);
        public Task<Client> FindClientAsync(Client clnt);
        public Task<EditReservationViewModel> EditReservationByIdAsync(string id);
        public Task UpdateReservationAsync(EditReservationViewModel model);
        public Task<DetailsReservationViewModel> GetReservationDetailsAsync(string id);
        public Task<DetailsReservationViewModel> GetReservationToDeleteAsync(string id);
        public Task DeleteReservationAsync(DetailsReservationViewModel model);
        public bool HasReservationPassed(DateTime LeaveDate);
    }
}

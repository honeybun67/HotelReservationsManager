using HotelReservationsManager.ViewModels.Clients;

namespace HotelReservationsManager.Services.Contracts
{
    public interface IClientsService
    {
        public Task<string> CreateClientAsync(CreateClientViewModel model);
    }
}

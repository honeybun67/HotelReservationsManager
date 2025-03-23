using HotelReservationsManager.ViewModels.Clients;

namespace HotelReservationsManager.Services.Contracts
{
    public interface IClientsService
    {
        public Task<string> CreateClientAsync(CreateClientViewModel model);

        public Task<ClientsIndexViewModel> GetClientsAsync(ClientsIndexViewModel model);

        public Task<EditClientViewModel> EditCustomerByIdAsync(string id);

        public Task UpdateCustomerAsync(EditClientViewModel model);

        public Task<ClientDetailsViewModel> DeleteClientByIdAsync(string id);

        public Task DeleteClientAsync(ClientDetailsViewModel model);
    }
}

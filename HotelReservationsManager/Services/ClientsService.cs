using HotelReservationsManager.Data;
using HotelReservationsManager.Data.Models;
using HotelReservationsManager.Services.Contracts;
using HotelReservationsManager.ViewModels.Clients;
using Microsoft.EntityFrameworkCore;

namespace HotelReservationsManager.Services
{
    public class ClientsService:IClientsService
    {
        private readonly ApplicationDbContext context;

        public ClientsService(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<string> CreateClientAsync(CreateClientViewModel model)
        {
            Client client = new Client()
            {
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                IsAdult = model.IsAdult,
                Number = model.PhoneNumber,
            };
            await this.context.Clients.AddAsync(client);
            await this.context.SaveChangesAsync();
            return client.Id;
        }

        public async Task<ClientsIndexViewModel> GetClientsAsync(ClientsIndexViewModel model)
        {
            if (model == null)
            {
                model = new ClientsIndexViewModel(0);
            }
            IQueryable<Client> dataClients = context.Clients;

            if (!string.IsNullOrWhiteSpace(model.FilterByName))
            {
                dataClients = dataClients.Where(x => x.FirstName.Contains(model.FilterByName) || x.LastName.Contains(model.FilterByName));
            }

            model.ElementsCount = await dataClients.CountAsync();

            model.Clients = await dataClients
                .Skip((model.Page - 1) * model.ItemsPerPage)
                .Take(model.ItemsPerPage)
                .Select(x => new ClientIndexViewModel()
                {
                    Id = x.Id,
                    Email = x.Email,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    IsAdult = x.IsAdult,
                    PhoneNumber = x.Number,
                })
                .ToListAsync();
            return model;
        }
    }
}


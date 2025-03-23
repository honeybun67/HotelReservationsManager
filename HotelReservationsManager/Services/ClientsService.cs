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

        public async Task<EditClientViewModel> EditCustomerByIdAsync(string id)
        {
            Client client = await this.context.Clients.FindAsync(id);
            if (client != null)
            {
                return new EditClientViewModel()
                {
                    Id = client.Id,
                    Email = client.Email,
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                    IsAdult = client.IsAdult,
                    PhoneNumber = client.Number,
                };
            }
            return null;
        }

        public async Task UpdateCustomerAsync(EditClientViewModel model)
        {
            Client client = new Client()
            {
                Id = model.Id,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                IsAdult = model.IsAdult,
                Number = model.PhoneNumber,
            };
            context.Update(client);
            await context.SaveChangesAsync();
        }

        public async Task<ClientDetailsViewModel> DeleteClientByIdAsync(string id)
        {
            Client client = await context.Clients.FindAsync(id);
            if (client != null)
            {
                ClientDetailsViewModel model = new ClientDetailsViewModel()
                {
                    Id = client.Id,
                    Email = client.Email,
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                    IsAdult = client.IsAdult,
                    PhoneNumber = client.Number,
                };
                return model;
            }
            return null;
        }

        public async Task DeleteClientAsync(ClientDetailsViewModel model)
        {
            Client client = await context.Clients.FindAsync(model.Id);
            if (client != null)
            {
                //if (client.ReservationId != null)
                //{
                //	client.ReservationId= null;
                //}
                context.Clients.Remove(client);
                await this.context.SaveChangesAsync();
            }
        }

    }
}


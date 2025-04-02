using HotelReservationsManager.Data.Models;
using HotelReservationsManager.Data;
using HotelReservationsManager.Services.Contracts;
using HotelReservationsManager.ViewModels.Clients;
using HotelReservationsManager.ViewModels.Rooms;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HotelReservationsManager.ViewModels.Reservations;

namespace HotelReservationsManager.Services
{
    public class ReservationsService : IReservationsService
    {
        private readonly ApplicationDbContext context;

        public ReservationsService(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task CreateReservationAsync(CreateReservationViewModel model)
        {
            User user = await context.Users.FindAsync(model.UserId);
            Room room = await context.Rooms.FindAsync(model.RoomId);

            List<Client> SelectedClients = model.Clients
                .Select(x => new Client
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Number = x.Number,
                }).ToList();

            //Create Reservation
            Reservation reservation = new Reservation()
            {
                User = user,
                AccommodationDate = model.AccommodationDate,
                EmptyDate = model.LeaveDate,
                AllInclusive = model.AllInclusive,
                WithBreakfast = model.WithBreakfast,
            };
            reservation.Clients = new List<Client>();

            reservation.Price = CalculatePriceWithExtras(model.WithBreakfast, model.AllInclusive);

            if (room.Id != reservation.RoomId)
            {
                Reservation roomres = await context.Reservations.FirstOrDefaultAsync(x => x.RoomId == room.Id);
                if (roomres == null && room.IsAvailable)
                {
                    ReserveRoom(room, reservation);
                }
            }

            //Add Reservation to database and save
            await this.context.Reservations.AddAsync(reservation);
            await this.context.SaveChangesAsync();

            foreach (var item in SelectedClients)
            {
                Client client = await FindClientAsync(item);
                if (client.Reservation == null && room.Capacity > reservation.Clients.Count)
                {
                    reservation.Price += CalculatePrice(model.LeaveDate, model.AccommodationDate, room, client);
                    await AddClientToReservationAsync(client, reservation);
                }
            }

            //Attach instance of Reservation
            context.Reservations.Attach(reservation);
            await this.context.SaveChangesAsync();
        }
        public async Task<IndexReservationsViewModel> GetReservationsAsync(IndexReservationsViewModel model)
        {
            model.Reservations = await context.Reservations
                .Skip((model.Page - 1) * model.ItemsPerPage)
                .Take(model.ItemsPerPage)
                .Select(x => new IndexReservationViewModel()
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    Room = context.Rooms.FirstOrDefault(y => y.Id == x.RoomId),
                    AccommodationDate = x.AccommodationDate,
                    EmptyDate = x.EmptyDate,
                    AllInclusive = x.AllInclusive,
                    WithBreakfast = x.WithBreakfast,
                    Price = x.Price,
                })
                .ToListAsync();
            model.ElementsCount = await this.context.Reservations.CountAsync();
            return model;
        }
        public async Task<DetailsReservationViewModel> GetReservationDetailsAsync(string id)
        {
            Reservation reservation = await this.context.Reservations.FindAsync(id);

            if (reservation != null)
            {
                DetailsReservationViewModel model = new DetailsReservationViewModel()
                {
                    Id = reservation.Id,
                    UserId = reservation.UserId,
                    AccommodationDate = reservation.AccommodationDate,
                    EmptyDate = reservation.EmptyDate,
                    AllInclusive = reservation.AllInclusive,
                    WithBreakfast = reservation.WithBreakfast,
                    Price = reservation.Price,
                };

                //Get Clients for Current Reservation
                model.Clients = await context.Clients
                    .Where(x => x.ReservationId == model.Id)
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
            return null;
        }
        public async Task<EditReservationViewModel> EditReservationByIdAsync(string id)
        {
            Reservation reservation = await context.Reservations.FindAsync(id);
            if (reservation != null)
            {
                EditReservationViewModel model = new EditReservationViewModel()
                {
                    Id = reservation.Id,
                    UserId = reservation.UserId,
                    RoomId = reservation.RoomId,
                    AccommodationDate = reservation.AccommodationDate,
                    EmptyDate = reservation.EmptyDate,
                    HasAllInclusive = reservation.AllInclusive,
                    HasBreakfast = reservation.WithBreakfast,
                };
                model.ClientsToRemove = reservation.Clients.Select(x => new ClientIndexViewModel()
                {
                    Id = x.Id,
                    Email = x.Email,
                    FirstName = x.FirstName,
                    IsAdult = x.IsAdult,
                    LastName = x.LastName,
                    PhoneNumber = x.Number,
                }).ToList();

                SelectList selectList = new SelectList(await GetAllRoomsSelectListAsync(model), "Id", "Number");
                model.Rooms = selectList;

                if (!string.IsNullOrEmpty(model.RoomId) && await GetRoomCapacityAsync(model.RoomId) > 0)
                {
                    model.RoomCapacity = await GetRoomCapacityAsync(model.RoomId);

                    for (int i = 0; i < model.RoomCapacity - reservation.Clients.Count; i++)
                    {
                        model.ClientsToAdd.Add(new Client());
                    }

                }
                return model;
            }
            return null;
        }
        public async Task UpdateReservationAsync(EditReservationViewModel model)
        {
            User user = await context.Users.FindAsync(model.UserId);
            Room room = await context.Rooms.FindAsync(model.RoomId);
            Reservation reservation = await context.Reservations.FindAsync(model.Id);

            reservation.User = user;

            if (room.Id != reservation.RoomId)
            {
                ReserveRoom(room, reservation);
            }

            reservation.AccommodationDate = model.AccommodationDate;
            reservation.EmptyDate = model.EmptyDate;

            reservation.AllInclusive = model.HasAllInclusive;
            reservation.WithBreakfast = model.HasBreakfast;

            reservation.Price = CalculatePriceWithExtras(model.HasBreakfast, model.HasAllInclusive);

            List<Client> ClientsToAdd = model.ClientsToAdd
            .Select(x => new Client
            {
                FirstName = x.FirstName,
                LastName = x.LastName,
                Number = x.Number,
            }).ToList();

            foreach (var clnt in ClientsToAdd)
            {
                Client Client = await FindClientAsync(clnt);
                if (Client != null && Client.ReservationId != reservation.Id)
                {
                    await AddClientToReservationAsync(Client, reservation);
                }
            }

            foreach (var clnt in model.ClientsToRemove)
            {
                Client Client = await context.Clients.FindAsync(clnt.Id);
                if (Client != null && Client.ReservationId == reservation.Id)
                {
                    await RemoveClientReservationAsync(Client, reservation);
                }
            }

            foreach (var item in reservation.Clients)
            {
                reservation.Price += CalculatePrice(model.EmptyDate, model.AccommodationDate, room, item);
            }

            context.Update(reservation);
            await context.SaveChangesAsync();
            if (!reservation.Clients.Any())
            {
                context.Remove(reservation);
                if (reservation.Room != null)
                {
                    reservation.Room.IsAvailable = true;
                    reservation.Room.Reservation = null;
                }
                await context.SaveChangesAsync();
            }
        }
        public async Task<DetailsReservationViewModel> GetReservationToDeleteAsync(string id)
        {
            Reservation reservation = await this.context.Reservations.FindAsync(id);
            if (reservation != null)
            {
                DetailsReservationViewModel model = new DetailsReservationViewModel()
                {
                    Id = reservation.Id,
                    AccommodationDate = reservation.AccommodationDate,
                    EmptyDate = reservation.EmptyDate,
                    UserId = reservation.UserId,
                    AllInclusive = reservation.AllInclusive,
                    WithBreakfast = reservation.WithBreakfast,
                    Price = reservation.Price,
                };


                model.Clients = await context.Clients
                    .Where(x => x.ReservationId == model.Id)
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
            return null;
        }
        public async Task DeleteReservationAsync(DetailsReservationViewModel model)
        {
            Reservation r = await this.context.Reservations.FindAsync(model.Id);
            if (r != null)
            {
                foreach (var client in r.Clients)
                {
                    client.Reservation = null;
                }

                if (r.Room != null)
                {
                    r.Room.IsAvailable = true;
                    r.Room.Reservation = null;
                }
                this.context.Reservations.Remove(r);
                await this.context.SaveChangesAsync();
            }
        }
        private static void ReserveRoom(Room room, Reservation reservation)
        {
            room.IsAvailable = false;
            if (reservation.Room != null)
            {
                reservation.Room.IsAvailable = true;
                reservation.Room.Reservation = null;
            }
            reservation.RoomId = room.Id;
            room.ReservationId = reservation.Id;
        }
        public async Task<List<SelectListRoomViewModel>> GetFreeRoomsSelectListAsync()
        {
            List<SelectListRoomViewModel> SelectList = await this.context.Rooms.Where(x => x.Reservation == null && x.IsAvailable).Select(x => new SelectListRoomViewModel()
            {
                Id = x.Id,
                Capacity = x.Capacity,
                RoomType = x.Type,
                PricePerAdultBed = x.PricePerAdultBed,
                PricePerChildBed = x.PricePerChildBed,
                Number = x.Number,
            })
                .ToListAsync();
            return SelectList;
        }
        public async Task<List<SelectListRoomViewModel>> GetAllRoomsSelectListAsync(EditReservationViewModel model)
        {
            List<SelectListRoomViewModel> SelectList = await GetFreeRoomsSelectListAsync();
            Room room = await context.Rooms.FirstOrDefaultAsync(x => x.Id == model.RoomId);
            SelectListRoomViewModel rsvm = new SelectListRoomViewModel()
            {
                Id = room.Id,
                Capacity = room.Capacity,
                RoomType = room.Type,
                Number = room.Number,
                PricePerAdultBed = room.PricePerAdultBed,
                PricePerChildBed = room.PricePerChildBed,
            };
            SelectList.Add(rsvm);
            return SelectList;
        }
        private async Task RemoveClientReservationAsync(Client dbClient, Reservation res)
        {
            var existingReservation = context.Reservations.Local.SingleOrDefault(o => o.Id == res.Id);
            if (existingReservation != null)
                context.Entry(existingReservation).State = EntityState.Modified;

            dbClient.Reservation = null;

            context.Update(dbClient);
            await context.SaveChangesAsync();
        }
        private async Task AddClientToReservationAsync(Client dbClient,
            Reservation reservation)
        {
            dbClient.Reservation = reservation;

            ClientHistory ch = new ClientHistory()
            {
                Client = dbClient,
                AccomodationDate = reservation.AccommodationDate,
                LeaveDate = reservation.EmptyDate,
                ResPrice = reservation.Price,
                ResRoomNumber = reservation.Room.Number,
            };

            context.ClientHistories.Add(ch);

            context.Reservations.Attach(reservation);
            await this.context.SaveChangesAsync();
        }
        public async Task<int> GetRoomCapacityAsync(string id)
        {
            Room room = await context.Rooms.FindAsync(id);
            return room.Capacity;
        }
        public async Task<Client> FindClientAsync(Client cust)
        {
            Client Client = await context.Clients.FirstOrDefaultAsync(x => x.FirstName == cust.FirstName && x.LastName == cust.LastName && x.Number == cust.Number);
            if (Client != null)
            {
                return Client;
            }
            else
            {
                return null;
            }
        }
        private double CalculatePriceWithExtras(Boolean HasBreakfast, Boolean HasAllInclusive)
        {
            double price = new double();
            if (HasBreakfast)
            {
                price += 150;
            }
            if (HasAllInclusive)
            {
                price += 300;
            }
            return price;
        }
        private double CalculatePrice(DateTime leaveDate, DateTime accomdate, Room room, Client cust)
        {

            TimeSpan duration = leaveDate - accomdate;
            if (leaveDate == accomdate)
            {
                duration = TimeSpan.FromDays(1);
            }
            double price = new double();

            Client dbc = context.Clients.Find(cust.Id);
            if (dbc.IsAdult)
            {
                price += room.PricePerAdultBed * (double)duration.TotalDays;
            }
            if (!dbc.IsAdult)
            {
                price += room.PricePerChildBed * (double)duration.TotalDays;
            }
            return price;
        }
        public bool HasReservationPassed(DateTime LeaveDate)
        {
            if (LeaveDate <= DateTime.Now)
            {
                return true;
            }
            return false;
        }


    }
}


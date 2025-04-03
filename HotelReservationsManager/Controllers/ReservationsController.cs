namespace HotelReservationsManager.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using HotelReservationsManager.Data;
    using HotelReservationsManager.Data.Models;
    using HotelReservationsManager.ViewModels.Reservations;
    using HotelReservationsManager.Services.Contracts;
    using System.Security.Claims;
    using Microsoft.AspNetCore.Authorization;
    using HotelReservationsManager.ViewModels;
    using HotelReservationsManager.Data.Models;
    using HotelReservationsManager.Data;
    using HotelReservationsManager.Services.Contracts;
    using HotelReservationsManager.ViewModels.Reservations;
    using HotelReservationsManager.ViewModels;

    [Authorize(Roles = "Admin,User")]
    public class ReservationsController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IReservationsService service;

        public ReservationsController(IReservationsService service, ApplicationDbContext context)
        {
            this.service = service;
            this.context = context;
        }

        // GET: Reservations
        public async Task<IActionResult> Index(IndexReservationsViewModel model)
        {
            model = await service.GetReservationsAsync(model);
            return View(model);
        }

        public async Task<IActionResult> Create(string roomId)
        {
            CreateReservationViewModel model = new CreateReservationViewModel();
            await ConfigureCreateVM(model, roomId);
            if (!model.Rooms.Any())
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel() { ErrorMessage = "No free rooms at his time" });
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateReservationViewModel model)
        {
            //Gets current user's id
            model.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            //Remove temporary empty Client objects
            model.Clients = model.Clients.Where(x => x.FirstName != null && x.LastName != null && x.Number != null).ToList();

            // Check if user submitted a roomid
            if (model.RoomId == null)
            {
                ModelState.AddModelError(nameof(model.RoomId), "Please select and submit a room");
                await ConfigureCreateVM(model, model.RoomId);
                return View(model);
            }
            //Checks Accommodation and Leave date if they are sensible
            if (CheckDurationOfDates(model.LeaveDate, model.AccommodationDate))
            {
                ModelState.AddModelError(nameof(model.LeaveDate), "Leave date can't be before Accommodation Date");
                ModelState.AddModelError(nameof(model.AccommodationDate), "Accommodation Date can't be after Leave Date");
                await ConfigureCreateVM(model, model.RoomId);
                return View(model);
            }
            //Check if nubmer of people is more than room capacity
            if (await service.GetRoomCapacityAsync(model.RoomId) < model.Clients.Count)
            {
                ModelState.AddModelError(nameof(model.Clients), "Number of people exceeds Room Capacity");
                await ConfigureCreateVM(model, model.RoomId);
                return View(model);
            }
            //check if user inputed at least 1 Client
            if (!model.Clients.Any())
            {
                ModelState.AddModelError(nameof(model.Clients), "Add at least 1 person");
                await ConfigureCreateVM(model, model.RoomId);
                return View(model);
            }
            List<Client> inputClients = model.Clients.Select(x => new Client()
            {
                FirstName = x.FirstName,
                LastName = x.LastName,
                Number = x.Number,
            }).ToList();
            //chek every inputted User if he exists in database and if he already has a reservation
            foreach (var cust in inputClients)
            {
                Client Client = await service.FindClientAsync(cust);
                if (Client == null)
                {
                    ModelState.AddModelError(nameof(model.Clients), $"{cust.FirstName} {cust.LastName} isn't found in the database. You have to first add him/her");
                    await ConfigureCreateVM(model, model.RoomId);
                    return View(model);
                }
                if (Client.Reservation != null)
                {
                    ModelState.AddModelError(nameof(model.Clients), $"{cust.FirstName} {cust.LastName} has already been asigned to a Reservation");
                    await ConfigureCreateVM(model, model.RoomId);
                    return View(model);
                }
            }
            ModelState.MarkFieldValid("Reservations");
            await service.CreateReservationAsync(model);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(string id)
        {
            DetailsReservationViewModel model = await service.GetReservationDetailsAsync(id);
            return View(model);
        }
        // GET: Reservations/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            EditReservationViewModel model = await service.EditReservationByIdAsync(id);
            return View(model);
        }

        // POST: Reservations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditReservationViewModel model)
        {
            model.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            //Remove temporary empty Customer objects
            model.ClientsToAdd = model.ClientsToAdd.Where(x => x.FirstName != null && x.LastName != null && x.Number != null).ToList();
            model.ClientsToRemove = model.ClientsToRemove.Where(x => x.RemoveFromRes).ToList();

            // Check if user submitted a roomid
            if (model.RoomId == null)
            {
                ModelState.AddModelError(nameof(model.RoomId), "Please select and submit a room");
                return View(await service.EditReservationByIdAsync(model.Id));
            }
            //Checks Accommodation and Leave date if they are sensible
            if (CheckDurationOfDates(model.EmptyDate, model.AccommodationDate))
            {
                ModelState.AddModelError(nameof(model.EmptyDate), "Leave date can't be before Accommodation Date");
                ModelState.AddModelError(nameof(model.AccommodationDate), "Accommodation Date can't be after Leave Date");
                return View(await service.EditReservationByIdAsync(model.Id));
            }
            //Check if nubmer of people is more than room capacity
            if (await service.GetRoomCapacityAsync(model.RoomId) < model.ClientsToAdd.Count)
            {
                ModelState.AddModelError(nameof(model.ClientsToAdd), "Number of people exceeds Room Capacity");
                return View(await service.EditReservationByIdAsync(model.Id));
            }
            List<Client> inputCustomers = model.ClientsToAdd.Select(x => new Client()
            {
                FirstName = x.FirstName,
                LastName = x.LastName,
                Number = x.Number,
            }).ToList();
            //chek every inputted User if he exists in database and if he already has a reservation
            foreach (var clnt in inputCustomers)
            {
                Client client = await service.FindClientAsync(clnt);
                if (client == null)
                {
                    ModelState.AddModelError(nameof(model.ClientsToAdd), $"{clnt.FirstName} {clnt.LastName} isn't found in the database. You have to first add him/her");
                    return View(await service.EditReservationByIdAsync(model.Id));
                }
                if (client.Reservation != null)
                {
                    ModelState.AddModelError(nameof(model.ClientsToAdd), $"{clnt.FirstName} {clnt.LastName} has already been asigned to a Reservation");
                    return View(await service.EditReservationByIdAsync(model.Id));
                }
            }
            await service.UpdateReservationAsync(model);

            return RedirectToAction(nameof(Index));
        }

        // GET: Reservations/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            DetailsReservationViewModel model = await service.GetReservationToDeleteAsync(id);
            return View(model);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(DetailsReservationViewModel model)
        {
            await service.DeleteReservationAsync(model);
            return RedirectToAction(nameof(Index));
        }

        private bool ReservationExists(string id)
        {
            return context.Reservations.Any(x => x.Id == id);
        }
        private async Task ConfigureCreateVM(CreateReservationViewModel model, string roomId)
        {
            model.Rooms = new SelectList(await service.GetFreeRoomsSelectListAsync(), "Id", "Number");
            if (!string.IsNullOrWhiteSpace(roomId) && await service.GetRoomCapacityAsync(roomId) > 0)
            {
                model.RoomId = roomId;
                model.RoomCapacity = await service.GetRoomCapacityAsync(roomId);
            }
            for (int i = 0; i < model.RoomCapacity; i++)
            {
                model.Clients.Add(new Client());
            }
        }
        private static bool CheckDurationOfDates(DateTime LeaveDate, DateTime AccommodationDate)
        {
            return LeaveDate < AccommodationDate;
        }
    }
}
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

        public async Task<IActionResult> Index(IndexReservationsViewModel model)
        {
            model = await service.GetReservationsAsync(model);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create(string roomId)
        {
            var model = new CreateReservationViewModel();
            await ConfigureCreateVM(model, roomId);

            if (!model.Rooms.Any())
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { ErrorMessage = "No free rooms at this time" });
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateReservationViewModel model)
        {
            model.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            model.Clients = model.Clients.Where(x => x.FirstName != null && x.LastName != null && x.Number != null).ToList();

            if (string.IsNullOrEmpty(model.RoomId))
            {
                ModelState.AddModelError(nameof(model.RoomId), "Please select and submit a room");
                await ConfigureCreateVM(model, model.RoomId);
                return View(model);
            }

            if (CheckDurationOfDates(model.LeaveDate, model.AccommodationDate))
            {
                ModelState.AddModelError(nameof(model.LeaveDate), "Leave date can't be before Accommodation Date");
                ModelState.AddModelError(nameof(model.AccommodationDate), "Accommodation Date can't be after Leave Date");
                await ConfigureCreateVM(model, model.RoomId);
                return View(model);
            }

            if (await service.GetRoomCapacityAsync(model.RoomId) < model.Clients.Count)
            {
                ModelState.AddModelError(nameof(model.Clients), "Number of people exceeds Room Capacity");
                await ConfigureCreateVM(model, model.RoomId);
                return View(model);
            }

            if (!model.Clients.Any())
            {
                ModelState.AddModelError(nameof(model.Clients), "Add at least 1 person");
                await ConfigureCreateVM(model, model.RoomId);
                return View(model);
            }

            foreach (var cust in model.Clients)
            {
                var client = await service.FindClientAsync(cust);
                if (client == null)
                {
                    ModelState.AddModelError(nameof(model.Clients), $"{cust.FirstName} {cust.LastName} isn't found in the database. You have to first add him/her");
                    await ConfigureCreateVM(model, model.RoomId);
                    return View(model);
                }
                if (client.Reservation != null)
                {
                    ModelState.AddModelError(nameof(model.Clients), $"{cust.FirstName} {cust.LastName} has already been assigned to a Reservation");
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
            var model = await service.GetReservationDetailsAsync(id);
            return View("ReservationDetails", model);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var model = await service.EditReservationByIdAsync(id);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditReservationViewModel model)
        {
            model.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            model.ClientsToAdd = model.ClientsToAdd.Where(x => x.FirstName != null && x.LastName != null && x.Number != null).ToList();
            model.ClientsToRemove = model.ClientsToRemove.Where(x => x.RemoveFromRes).ToList();

            if (string.IsNullOrEmpty(model.RoomId))
            {
                ModelState.AddModelError(nameof(model.RoomId), "Please select and submit a room");
                return View(await service.EditReservationByIdAsync(model.Id));
            }

            if (CheckDurationOfDates(model.LeaveDate, model.AccommodationDate))
            {
                ModelState.AddModelError(nameof(model.LeaveDate), "Leave date can't be before Accommodation Date");
                ModelState.AddModelError(nameof(model.AccommodationDate), "Accommodation Date can't be after Leave Date");
                return View(await service.EditReservationByIdAsync(model.Id));
            }

            if (await service.GetRoomCapacityAsync(model.RoomId) < model.ClientsToAdd.Count)
            {
                ModelState.AddModelError(nameof(model.ClientsToAdd), "Number of people exceeds Room Capacity");
                return View(await service.EditReservationByIdAsync(model.Id));
            }

            foreach (var clnt in model.ClientsToAdd)
            {
                var client = await service.FindClientAsync(clnt);
                if (client == null)
                {
                    ModelState.AddModelError(nameof(model.ClientsToAdd), $"{clnt.FirstName} {clnt.LastName} isn't found in the database. You have to first add him/her");
                    return View(await service.EditReservationByIdAsync(model.Id));
                }
                if (client.Reservation != null)
                {
                    ModelState.AddModelError(nameof(model.ClientsToAdd), $"{clnt.FirstName} {clnt.LastName} has already been assigned to a Reservation");
                    return View(await service.EditReservationByIdAsync(model.Id));
                }
            }

            await service.UpdateReservationAsync(model);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var model = await service.GetReservationToDeleteAsync(id);
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var model = await service.GetReservationToDeleteAsync(id);
            if (model == null)
            {
                return NotFound();
            }

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

        private static bool CheckDurationOfDates(DateTime leaveDate, DateTime accommodationDate)
        {
            return leaveDate < accommodationDate;
        }
    }
}

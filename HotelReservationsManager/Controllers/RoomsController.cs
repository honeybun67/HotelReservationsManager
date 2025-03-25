using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HotelReservationsManager.Data;
using HotelReservationsManager.Data.Models;
using HotelReservationsManager.Data.Models.Enum;
using HotelReservationsManager.Services.Contracts;
using HotelReservationsManager.ViewModels.Rooms;
using Microsoft.AspNetCore.Authorization;

namespace HotelReservationsManager.Controllers
{
    public class RoomsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IRoomsService service;

        public RoomsController(ApplicationDbContext context, IRoomsService service)
        {
            _context = context;
            this.service = service;
        }

        // GET: Rooms
        public async Task<IActionResult> Index(IndexRoomsViewModel model)
        {
            model = await service.GetRoomsAsync(model);
            return View(model);
        }

        // GET: Rooms/Details/5
        public async Task<IActionResult> Details(string id)
        {
            RoomDetailsViewModel model = await service.GetRoomDetailsAsync(id);
            return View(model);
        }

        // GET: Rooms/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Rooms/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateRoomViewModel model)
        {
            if (ModelState.IsValid)
            {
                await service.CreateRoomAsync(model);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Rooms/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id)
        {
            EditRoomViewModel model = await service.EditRoomAsync(id);
            return View(model);
        }

        // POST: Rooms/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditRoomViewModel model)
        {
            await service.UpdateRoomAsync(model);
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Seed()
        {

            Random random = new Random();
            for (int i = 1; i <= 20; i++)
            {
                string result = await service.CreateRoomAsync(

                      new CreateRoomViewModel()
                      {
                          Capacity = random.Next(1, 6),
                          Number = i + 900,
                          RoomType = (RoomType)random.Next(Enum.GetNames(typeof(RoomType)).Length),
                          IsAvailable = random.Next(0, 2) == 0,
                          PricePerAdultBed = random.Next(200, 400),
                          PricePerChildBed = random.Next(100, 300)
                      }
                      );
            }
            return RedirectToAction(nameof(Index));
        }
        // GET: Rooms/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            RoomDetailsViewModel model = await service.DeleteRoomByIdAsync(id);
            return View(model);
        }

        // POST: Rooms/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(RoomDetailsViewModel model)
        {
            await service.DeleteConfirmRoomAsync(model);
            return RedirectToAction(nameof(Index));
        }

        private bool RoomExists(string id)
        {
            return _context.Rooms.Any(e => e.Id == id);
        }
    }
}

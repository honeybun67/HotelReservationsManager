namespace HotelReservations.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using HotelReservationsManager.Data;
    using HotelReservationsManager.ViewModels.Clients;
    using HotelReservationsManager.Services.Contracts;
    using Microsoft.AspNetCore.Authorization;
    using HotelReservationsManager.Data;
    using HotelReservationsManager.Services.Contracts;
    using HotelReservationsManager.ViewModels.Clients;

    [Authorize(Roles = "Admin,User")]
    public class ClientsController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IClientsService service;

        public ClientsController(ApplicationDbContext context, IClientsService service)
        {
            this.context = context;
            this.service = service;
        }

        // GET: Clients
        public async Task<IActionResult> Index(ClientsIndexViewModel model)
        {
            model = await service.GetClientsAsync(model);

            return View(model);
        }

        // GET: Clients/Details/5
        public async Task<IActionResult> Details(string id, ClientDetailsViewModel model)
        {
            model = await service.GetClientDetailsByIdAsync(id);
            return View(model);

        }
        public async Task<IActionResult> Seed()
        {
            List<string> firstName = new List<string>() { "John", "William", "Sara", "Tony", "Jane", "Carl", "Ben", "Lusy" };
            List<string> lastName = new List<string>() { "Johnson", "Phillips", "Benet", "Howard", "Willies", "Perry" };
            Random random = new Random();
            Boolean isAdult = false;
            for (int i = 0; i < firstName.Count; i++)
            {
                if (firstName[i].Length > 3)
                {
                    isAdult = true;
                }
            }
            for (int i = 1; i <= 20; i++)
            {
                string result = await service.CreateClientAsync(

                      new CreateClientViewModel()
                      {
                          FirstName = $"{firstName[random.Next(0, firstName.Count)]}",
                          LastName = $"{lastName[random.Next(0, lastName.Count)]}",
                          PhoneNumber = random.Next(0870000000, 0899999999).ToString("D10"),
                          IsAdult = isAdult,
                          Email = $"client{i}@gmail.com"
                      }
                      );
            }
            return RedirectToAction(nameof(Index));
        }
        // GET: Clients/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateClientViewModel model)
        {
            if (ModelState.IsValid)
            {
                await service.CreateClientAsync(model);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Clients/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            EditClientViewModel model = await service.EditCustomerByIdAsync(id);
            return View(model);
        }

        // POST: Clients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditClientViewModel model)
        {
            if (ModelState.IsValid)
            {
                await service.UpdateCustomerAsync(model);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Clients/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            ClientDetailsViewModel model = await service.DeleteClientByIdAsync(id);
            return View(model);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(ClientDetailsViewModel model)
        {
            await service.DeleteClientAsync(model);
            return RedirectToAction(nameof(Index));
        }

        private bool ClientExists(string id)
        {
            return context.Clients.Any(e => e.Id == id);
        }
    }
}
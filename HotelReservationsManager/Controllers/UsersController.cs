using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HotelReservationsManager.Data;
using HotelReservationsManager.Data.Models;
using HotelReservationsManager.ViewModels.User;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using HotelReservationsManager.Services.Contracts;

namespace HotelReservationsManager.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUserService service;

        public UsersController(IUserService service)
        {
            this.service = service;
        }

        [Authorize(Roles = GlobalConstants.AdminRole)]
        public async Task<IActionResult> Index(IndexUsersViewModel? model)
        {
            model = await service.GetUsersAsync(model);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                await service.CreateUserAsync(model);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [Authorize(Roles = GlobalConstants.AdminRole)]
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var model = await service.GetUserToEditAsync(id);
            return View(model);
        }

        [Authorize(Roles = GlobalConstants.AdminRole)]
        [HttpPost]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                await service.UpdateUserAsync(model);
                return this.RedirectToAction(nameof(Index));
            }
            return View(model);
        }
        public async Task<IActionResult> Details(string id)
        {
            DetailsUserViewModel model = await service.GetUserDetailsAsync(id);
            return View(model);
        }
        [Authorize(Roles = GlobalConstants.AdminRole)]
        [HttpGet]
        public async Task<IActionResult> Seed()
        {
            const string Password = "123456";
            const string UCN = "1122334455";
            const string PhoneNumber = "0896342517";
            for (int i = 0; i < 50; i++)
            {
                string result = await service.CreateUserAsync(

                      new CreateUserViewModel()
                      {
                          FirstName = $"Name {i}",
                          MiddleName = $"MiddleName {i}",
                          LastName = $"LastName {i}",
                          UCN = UCN,
                          PhoneNumber = PhoneNumber,
                          HireDate = DateTime.UtcNow,
                          Password = Password,
                          ConfirmPassword = Password,
                          Email = $"user{i}@app.bg"
                      }
                      );
            }
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = GlobalConstants.AdminRole)]
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (id != userId)
            {
                await service.DeleteUserAsync(id);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            string returnUrl = Url.Content("~/");


            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await service.Login(model);
                if (result.Succeeded)
                {
                    // _logger.LogInformation("User logged in.");
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    // _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return RedirectToAction(nameof(Index), "Home");
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelReservationsManager.Common;
using HotelReservationsManager.Data;
using HotelReservationsManager.Data.Models;
using HotelReservationsManager.Services.Contracts;
using HotelReservationsManager.ViewModels.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;
using HotelReservationsManager.ViewModels.Users;
using HotelReservationsManager.Common;

namespace HotelReservationsManager.Services
{

    public class UsersService : IUsersService
    {
        private readonly UserManager<User> userManager;
        private readonly ApplicationDbContext context;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly SignInManager<User> signInManager;
        private const int ItemsCount = 0;

        public UsersService(UserManager<User> userManager, ApplicationDbContext context, RoleManager<IdentityRole> roleManager, SignInManager<User> signInManager)
        {
            this.userManager = userManager;
            this.context = context;
            this.roleManager = roleManager;
            this.signInManager = signInManager;
        }

        public async Task<string> CreateUserAsync(CreateUserViewModel model)
        {
            User user = new User()
            {
                FirstName = model.FirstName,
                MiddleName = model.MiddleName,
                LastName = model.LastName,
                UCN = model.UCN,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                HireDate = model.HireDate,
                UserName = model.Email,
                Status = model.Status,
            };

            var result = await userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                if (userManager.Users.Count() <= 1)
                {
                    IdentityRole roleUser = new IdentityRole() { Name = GlobalConstants.UserRole };
                    IdentityRole roleAdmin = new IdentityRole() { Name = GlobalConstants.AdminRole };
                    await roleManager.CreateAsync(roleUser);
                    await roleManager.CreateAsync(roleAdmin);
                    await userManager.AddToRoleAsync(user, GlobalConstants.AdminRole);
                }
                else
                {
                    await userManager.AddToRoleAsync(user, GlobalConstants.UserRole);
                }
            }
            return user.Id;
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            User? user = await GetUserByIdAsync(id);
            if (user != null)
            {
                var result = await userManager.DeleteAsync(user);
                return result.Succeeded;
            }
            return false;
        }



        public async Task<DetailsUserViewModel?> GetUserDetailsAsync(string id)
        {
            DetailsUserViewModel result = null;

            User user = await GetUserByIdAsync(id);

            if (user != null)
            {
                string roles = string.Join(", ", await userManager.GetRolesAsync(user));

                result = new DetailsUserViewModel()
                {
                    Id = user.Id,
                    Name = $"{user.FirstName} {user.MiddleName} {user.LastName}",
                    Email = user.Email != null ? user.Email : "n/a",
                    UCN = user.UCN,
                    Status = user.Status,
                    HireDate = user.HireDate,
                    PhoneNumber = user.PhoneNumber != null ? user.PhoneNumber : "n/a",
                    Role = roles
                };
            }

            return result;
        }

        public async Task<IndexUsersViewModel> GetUsersAsync(IndexUsersViewModel model)
        {
            if (model == null)
            {
                model = new IndexUsersViewModel(0);
            }

            IQueryable<User> dataUsers = userManager.Users;

            if (!string.IsNullOrWhiteSpace(model.FilterByName))
            {
                dataUsers = dataUsers.Where(x => x.FirstName.Contains(model.FilterByName) || x.MiddleName.Contains(model.FilterByName) || x.LastName.Contains(model.FilterByName));
            }

            model.ElementsCount = await dataUsers.CountAsync();

            model.Users = await dataUsers
                .Skip((model.Page - 1) * model.ItemsPerPage)
                .Take(model.ItemsPerPage)
                .Select(x => new IndexUserViewModel()
                {
                    Id = x.Id,
                    Name = $"{x.FirstName} {x.MiddleName} {x.LastName}",
                    Email = x.Email,
                    PhoneNumber = x.PhoneNumber,
                    Status = x.Status,
                    HireDate = x.HireDate,
                    UCN = x.UCN,
                    Role = string.Join(", ", userManager.GetRolesAsync(x).GetAwaiter().GetResult())
                })
                .ToListAsync();

            return model;
        }

        public async Task<int> GetUsersCountAsync()
        {
            return await userManager.Users.CountAsync();
        }

        public async Task<EditUserViewModel?> GetUserToEditAsync(string id)
        {
            EditUserViewModel? result = null;

            User? user = await GetUserByIdAsync(id);

            if (user != null)
            {
                var userRole = (await userManager.GetRolesAsync(user)).FirstOrDefault();
                result = new EditUserViewModel()
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Status = user.Status,
                    Role = userRole == "User",
                };
            }

            return result;


        }

        public async Task<string> UpdateUserAsync(EditUserViewModel user)
        {
            User? oldUser = await GetUserByIdAsync(user.Id);

            if (user.Role)
            {
                // If checked, add the user to the "User" role
                if (!(await userManager.IsInRoleAsync(oldUser, "User")))
                {
                    await userManager.AddToRoleAsync(oldUser, "User");
                }
            }
            else
            {
                // If unchecked, remove the user from the "User" role
                if (await userManager.IsInRoleAsync(oldUser, "User"))
                {
                    await userManager.RemoveFromRoleAsync(oldUser, "User");
                }
            }

            return user.Id;
        }

        private async Task<User?> GetUserByIdAsync(string id)
        {
            return await userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task Logout()
        {
            await signInManager.SignOutAsync();
        }

        public async Task<SignInResult> Login(LoginViewModel model)
        {
            return await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
        }
    }
}

﻿using HotelReservationsManager.ViewModels.Users;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelReservationsManager.Services.Contracts
{
    public interface IUsersService
    {
        public Task<string> CreateUserAsync(CreateUserViewModel model);

        public Task<bool> DeleteUserAsync(string id);

        public Task<IndexUsersViewModel> GetUsersAsync(IndexUsersViewModel users);

        public Task<int> GetUsersCountAsync();

        public Task<DetailsUserViewModel?> GetUserDetailsAsync(string id);

        public Task<EditUserViewModel?> GetUserToEditAsync(string id);

        public Task<string> UpdateUserAsync(EditUserViewModel user);

        public Task Logout();

        public Task<SignInResult> Login(LoginViewModel model);
    }
}

using HotelReservationsManager.ViewModels.User;
using Microsoft.AspNetCore.Identity;

namespace HotelReservationsManager.Services.Contracts
{
    public interface IUserService
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

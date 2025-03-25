using HotelReservationsManager.ViewModels.Rooms;

namespace HotelReservationsManager.Services.Contracts
{
    public interface IRoomsService
    {
        public Task<string> CreateRoomAsync(CreateRoomViewModel model);
        public Task<IndexRoomsViewModel> GetRoomsAsync(IndexRoomsViewModel model);
        public Task<RoomDetailsViewModel> GetRoomDetailsAsync(string id);
        public Task<EditRoomViewModel> EditRoomAsync(string id);
        public Task<string> UpdateRoomAsync(EditRoomViewModel model);
        public Task<RoomDetailsViewModel> DeleteRoomByIdAsync(string id);
        public Task DeleteConfirmRoomAsync(RoomDetailsViewModel model);
    }
}

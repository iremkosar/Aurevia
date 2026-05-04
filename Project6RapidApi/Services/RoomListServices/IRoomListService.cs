namespace Project6RapidApi.Services.RoomListServices
{
    public interface IRoomListService
    {
        Task<List<string>> GetRoomPhotosAsync(int hotelId, DateTime checkin, DateTime checkout, int adults, int rooms);
    }
}

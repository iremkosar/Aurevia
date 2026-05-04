using Project6RapidApi.ViewModels;

namespace Project6RapidApi.Services.HotelServices
{
    public interface IHotelDetailService
    {
        Task<StayDetailVm> GetHotelDetailAsync(
            int hotelId,
            DateTime checkin,
            DateTime checkout,
            int adults,
            int children,
            int rooms);
    }
}

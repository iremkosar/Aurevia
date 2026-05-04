using Project6RapidApi.ViewModels;

namespace Project6RapidApi.Services.HotelServices
{
    public interface IHotelSearchService
    {
        Task<List<PropertyCardVm>> SearchHotelsAsync(
            string destId,
            string searchType,
            DateTime checkin,
            DateTime checkout,
            int adults,
            int children,
            int rooms);
    }
}
